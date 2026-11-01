# SSO compartilhado (Keycloak)

Identidade única para todos os projetos de `csharp-studies`. Uma conta criada aqui vale em
qualquer app plugado neste realm, e o login feito num app é reconhecido nos outros.

| | |
|---|---|
| Admin console | http://localhost:8082 (`admin` / `admin`) |
| Realm | `studies` |
| Issuer (público) | `http://localhost:8082/realms/studies` |
| Host interno (entre containers) | `http://keycloak:8080` |
| Rede | `shared-sso` (externa) |
| Banco | Postgres, volume `sso_keycloak-db-data` |

## Subir

```bash
docker network create shared-sso   # uma vez só
cp .env.example .env               # e preencher com as credenciais do Google
docker compose up -d
```

Suba isto **antes** dos apps. Não é rígido: as APIs buscam JWKS de forma preguiçosa, na
primeira validação de token, então uma API que suba antes se recupera sozinha.

## Por que um realm só

Realms são isolados entre si — dois realms no mesmo Keycloak dão tanta SSO quanto dois
Keycloaks separados (nenhuma). Por isso todos os apps compartilham o realm `studies`, cada um
com **seu próprio client**, e a separação entre eles é feita por *audience*: o token emitido
para o `todoforsso-frontend` traz `aud: todoforsso-api` e é rejeitado com 401 pela API do
KShop, que espera `aud: kshop-api`. Identidade compartilhada, autorização separada.

## Os dois caminhos de login (e por que importam)

Cada SPA oferece login por senha **e** um botão de redirect. Eles não são equivalentes:

| | Onde acontece | Cria sessão SSO? |
|---|---|---|
| Form de usuário/senha (direct grant) | na própria tela do app | **Não** |
| Botão "Entrar com SSO" / "Entrar com Google" | tela do Keycloak | **Sim** |

O direct grant é uma chamada *backchannel* — o Keycloak emite tokens mas nunca escreve o cookie
de sessão no navegador, porque não houve navegação até o domínio dele. Só fluxo interativo
(authorization code) estabelece a sessão que os outros apps enxergam.

Consequência prática: entrando pelo **form**, o outro app vai pedir credencial de novo. Entrando
pelo **botão**, o outro app entra direto. Isso é comportamento correto do OIDC, não bug.

> Dá para eliminar essa assimetria movendo a tela de login para o Keycloak, com tema próprio por
> aplicação — passo a passo em [docs/tema-login-customizado.md](docs/tema-login-customizado.md).

## Plugar um projeto novo

1. **Client no realm** — adicionar em `deploy/keycloak/studies-realm.json.template`, copiando o
   bloco de `kshop-frontend`: trocar `clientId`, a porta em `redirectUris`, e o
   `included.client.audience` do protocol mapper para a audience da nova API.
2. **Compose do projeto** — não declarar serviço `keycloak`; anexar as APIs à rede externa:
   ```yaml
   services:
     minha-api:
       environment:
         - Keycloak__Authority=http://keycloak:8080/realms/studies   # interno, busca JWKS
         - Keycloak__Issuer=http://localhost:8082/realms/studies     # público, valida o "iss"
         - Keycloak__Audience=minha-api
       networks: [default, shared-sso]

   networks:
     shared-sso:
       external: true
   ```
   O split entre `Authority` e `Issuer` é intencional: o token que chega do navegador carrega o
   host público no `iss`, mas quem busca as chaves é o container, pela rede interna.
3. **Frontend** — `VITE_KEYCLOAK_URL=http://localhost:8082`, `VITE_KEYCLOAK_REALM=studies`,
   `VITE_KEYCLOAK_CLIENT_ID=<seu-client>`. Os `keycloak_service.ts` de TodoForSso e KShop são
   intercambiáveis; copie um deles.
4. **CORS** — apontar `Frontend__Origin` para a porta do front (o default é 5173 e silenciosamente
   bloqueia qualquer outra).
5. **Aplicar** — ver a armadilha abaixo.

Portas em uso: 5173 TodoForSso, 5174 KShop.

## Armadilha: `--import-realm` só importa uma vez

Com o Postgres persistente, o realm existe entre reinícios — e `--import-realm` **ignora
silenciosamente** o template quando o realm já existe. Editar
`studies-realm.json.template` depois do primeiro boot não tem efeito nenhum.

Para aplicar mudanças, uma das duas:

```bash
docker compose down -v && docker compose up -d   # recria do zero — APAGA todos os usuários
```

ou fazer a alteração pelo admin console, que persiste no Postgres. No dia a dia, o console é o
caminho certo; o template é o estado inicial, não a fonte da verdade contínua.

> `down -v` é o único comando aqui que perde dados. `down` sozinho é seguro.

## Google

O client OAuth vive no Google Cloud Console. A *authorized redirect URI* precisa ser o endpoint
de broker **deste realm**:

```
http://localhost:8082/realms/studies/broker/google/endpoint
```

Trocar o nome do realm muda essa URL — foi preciso adicioná-la ao migrar de `todoforsso` para
`studies`. `GOOGLE_CLIENT_ID`/`GOOGLE_CLIENT_SECRET` ficam em `.env` (fora do git) e são
injetados no template pelo serviço `keycloak-realm-init`, que resolve os placeholders com `sed`
antes do Keycloak subir.

## Roles

`user` (TodoForSso), `customer` (KShop), `admin` (compartilhada). Cada backend atribui a sua no
registro, então todas precisam existir no realm — o KShop, por exemplo, falha o cadastro com erro
se `customer` sumir.

**Login social não passa pelo registro das APIs**, que é quem normalmente atribui as roles. Sem
tratamento, a conta Google entraria sem role nenhuma. Por isso o IdP tem dois
`oidc-hardcoded-role-idp-mapper` (`GrantUserRole`, `GrantCustomerRole`) que concedem `user` e
`customer` automaticamente. Estão com `syncMode: FORCE`, ou seja, reaplicam a cada login — vale
inclusive para contas criadas antes do mapper existir.

`admin` nunca é concedida automaticamente; é atribuição manual pelo console.
