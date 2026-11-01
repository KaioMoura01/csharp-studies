# Tema de login customizado no Keycloak (template completo)

Objetivo: a tela de login passa a ser servida pelo Keycloak, mas com a cara de cada aplicação —
e o form de senha some das SPAs.

Por que isso vale a pena: hoje existem dois caminhos de login e só um propaga SSO. O form dentro
da SPA usa *direct grant*, que é uma chamada backchannel — o Keycloak emite tokens mas nunca
escreve o cookie de sessão no navegador. Com a tela toda no Keycloak, **qualquer** forma de entrar
(senha ou Google) passa pelo fluxo interativo e cria sessão SSO de verdade. A assimetria acaba.

## Como o Keycloak monta a tela

Dois arquivos importam:

| Arquivo | Papel |
|---|---|
| `template.ftl` | A **casca** de todas as páginas do fluxo de login (fundo, card, cabeçalho, área de mensagens). Define a macro `registrationLayout`. |
| `login.ftl` | O **conteúdo** da tela de login: o form e os botões sociais. Preenche as *sections* que a casca expõe. |

A casca expõe as sections via `<#nested "nome">`, e o `login.ftl` responde a elas com
`<#if section = "form">`. As que interessam: `header`, `form`, `socialProviders`, `info`.

Como o `template.ftl` é compartilhado por **todas** as páginas do fluxo (registro, esqueci a senha,
OTP), sobrescrevê-lo já dá identidade visual ao conjunto inteiro — não só ao login.

> Os templates originais estão dentro de
> `/opt/keycloak/lib/lib/main/org.keycloak.keycloak-themes-26.0.8.jar`. Vale extrair e usar como
> referência (é um zip comum). **Nunca edite os originais** — sempre crie um tema próprio.

## Passo 1 — Estrutura de arquivos

Um tema por aplicação, porque os visuais são diferentes:

```
sso/deploy/keycloak/themes/
├── todoforsso/
│   └── login/
│       ├── theme.properties
│       ├── template.ftl
│       ├── login.ftl
│       └── resources/css/styles.css
└── kshop/
    └── login/
        ├── theme.properties
        ├── template.ftl
        ├── login.ftl
        └── resources/css/styles.css
```

Montar no `sso/docker-compose.yml`, no serviço `keycloak`:

```yaml
volumes:
  - keycloak-realm-import:/opt/keycloak/data/import:ro
  - ./deploy/keycloak/themes:/opt/keycloak/themes:ro   # <- novo
```

Não precisa de build nem de jar: diretório montado já é reconhecido. E como o servidor roda em
`start-dev`, **temas não são cacheados** — você edita o CSS/FTL e dá F5.

## Passo 2 — `theme.properties`

```properties
parent=keycloak.v2
import=common/keycloak

styles=css/styles.css
stylesCommon=
```

`parent=keycloak.v2` faz você herdar as páginas que **não** vai sobrescrever (OTP, atualizar senha).
`stylesCommon=` vazio derruba o PatternFly — é o que te liberta do CSS deles para escrever o seu.

> **Consequência a conhecer:** sem PatternFly, as páginas herdadas que você não reescreveu ficam
> sem estilo, porque os templates delas emitem classes `pf-v5-*`. No seu caso isso é irrelevante:
> o registro é feito pela API da aplicação, não há reset de senha configurado e não há OTP. Se um
> dia habilitar algum desses, ou reescreve a página também, ou devolve o `stylesCommon` original.

## Passo 3 — `template.ftl` (a casca)

Versão enxuta, com a cara do TodoForSso (fundo escuro + card):

```ftl
<#macro registrationLayout bodyClass="" displayInfo=false displayMessage=true displayRequiredFields=false>
<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="robots" content="noindex, nofollow">
  <title>${msg("loginTitle",(realm.displayName!''))}</title>
  <#if properties.styles?has_content>
    <#list properties.styles?split(' ') as style>
      <link href="${url.resourcesPath}/${style}" rel="stylesheet" />
    </#list>
  </#if>
</head>
<body>
  <div class="page">
    <div class="card">
      <div class="card-header">
        <span class="brand-icon">&#10022;</span>
        <span class="brand-name">TodoForSso</span>
      </div>

      <h1 class="card-title"><#nested "header"></h1>

      <#if displayMessage && message?has_content>
        <div class="alert alert-${message.type}">
          ${kcSanitize(message.summary)?no_esc}
        </div>
      </#if>

      <#nested "form">
      <#nested "socialProviders">

      <#if displayInfo>
        <div class="card-footer"><#nested "info"></div>
      </#if>
    </div>
  </div>
</body>
</html>
</#macro>
```

## Passo 4 — `login.ftl` (o conteúdo)

Aqui os campos são `<input>` crus com as suas classes — não use as macros `@field.input`, que
emitem markup do PatternFly.

```ftl
<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username','password') displayInfo=realm.password && realm.registrationAllowed; section>

  <#if section = "header">
    Entre com sua conta

  <#elseif section = "form">
    <#if realm.password>
      <form id="kc-form-login" action="${url.loginAction}" method="post" novalidate="novalidate">
        <div class="field">
          <label for="username">Usuário</label>
          <input id="username" name="username" type="text" autofocus autocomplete="username"
                 value="${login.username!''}" />
        </div>

        <div class="field">
          <label for="password">Senha</label>
          <input id="password" name="password" type="password" autocomplete="current-password" />
        </div>

        <#if messagesPerField.existsError('username','password')>
          <span class="field-error">
            ${kcSanitize(messagesPerField.getFirstError('username','password'))?no_esc}
          </span>
        </#if>

        <button class="btn btn-primary" name="login" type="submit">Entrar</button>
      </form>
    </#if>

  <#elseif section = "socialProviders">
    <#if realm.password && social.providers?? && social.providers?has_content>
      <div class="divider"><span>ou</span></div>
      <#list social.providers as p>
        <a id="social-${p.alias}" class="btn btn-social" href="${p.loginUrl}">
          Entrar com ${p.displayName!}
        </a>
      </#list>
    </#if>

  <#elseif section = "info">
    <#if realm.password && realm.registrationAllowed>
      <span>Não tem conta? <a href="${url.registrationUrl}">Registre-se</a></span>
    </#if>
  </#if>

</@layout.registrationLayout>
```

### Variáveis que você tem à mão

| Variável | Para quê |
|---|---|
| `url.loginAction` | `action` do form — **obrigatório**, carrega o código de sessão |
| `url.registrationUrl` | link de cadastro do próprio Keycloak |
| `url.resourcesPath` | raiz de `resources/` do tema (imagens, css) |
| `realm.password` | se o realm aceita login por senha |
| `realm.registrationAllowed` / `realm.resetPasswordAllowed` | mostrar ou não os links |
| `social.providers` | lista de IdPs; cada um tem `alias`, `displayName`, `loginUrl` |
| `login.username` | repopula o campo após erro |
| `message.type` / `message.summary` | alerta global (erro, info, aviso) |
| `messagesPerField.existsError(...)` / `.getFirstError(...)` | erro por campo |
| `msg("chave")` | textos traduzidos |

Sempre passe conteúdo vindo do servidor por `kcSanitize(...)?no_esc` — é o que evita injeção de
HTML nas mensagens.

## Passo 5 — Apontar cada client para o seu tema

O realm é um só, mas o **login theme pode ser definido por client** — é isso que permite dois
visuais diferentes na mesma base de usuários.

No admin console (`localhost:8082`), para cada um:

- **Clients → `todoforsso-frontend` → Login theme →** `todoforsso`
- **Clients → `kshop-frontend` → Login theme →** `kshop`

Para deixar versionado, o equivalente no `studies-realm.json.template` é um atributo no client:

```json
"attributes": {
  "post.logout.redirect.uris": "+",
  "login_theme": "todoforsso"
}
```

> Lembre que `--import-realm` só importa se o realm ainda não existe. Com o Postgres persistente,
> mexer no template depois do primeiro boot não tem efeito — configure pelo console (que persiste)
> e deixe o template só como estado inicial.

## Passo 6 — Remover o form das SPAs

Com a tela no Keycloak, o form local vira código morto — e é justamente o caminho que não
propagava SSO.

### O que apagar

Nos **dois** front-ends:

- `keycloak_service.ts` → a função `login()` (password grant).
- `auth_store.ts` → a action `login(username, password)` do TodoForSso.
- KShop → `services/auth_service.ts` inteiro (era só o form) e `services/register_service.ts`,
  se você migrar o cadastro (ver abaixo).
- `LoginView.vue` → todo o form, os campos e o botão "Entrar".

### O que `LoginView.vue` vira

Uma tela que só redireciona. Não precisa nem renderizar:

```ts
onMounted(() => {
  redirectToLogin()   // KShop: redirectToIdpLogin()
})
```

### A simplificação que vem de brinde

Com o form fora, **o `prompt=none` deixa de ser necessário**. Ele existia para descobrir se havia
sessão *antes* de decidir mostrar o form. Agora não há decisão a tomar: o redirect normal já faz as
duas coisas — se existe sessão, o Keycloak devolve o `code` na hora, sem UI; se não existe, ele
mostra a tela de login. Um único caminho.

Então dá para remover `trySilentLogin()`, a flag `oidc_silent_tried` do `sessionStorage`, e o
tratamento de `login_required` no `CallbackView.vue`.

### Endurecer os clients

Sem password grant, desligue-o nos dois clients — some uma superfície de ataque:

**Clients → (cada um) → Settings → Direct access grants: OFF**

E no template: `"directAccessGrantsEnabled": false`.

## Passo 7 — Decidir o que fazer com o cadastro

Hoje o registro é feito pelas APIs (`POST /api/users/register` no TodoForSso,
`POST /api/auth/register` no KShop), que criam o usuário via Admin REST API e atribuem a role.

Duas saídas:

**(a) Manter como está.** As telas `/register` das SPAs continuam funcionando. Só o *login* migra.
Menor mudança.

**(b) Migrar para o Keycloak.** Ativa `Realm settings → Login → User registration`, e o link
"Registre-se" do `login.ftl` passa a levar à tela nativa. Para os usuários novos nascerem com as
roles certas, adicione `user` e `customer` às **default roles** do realm
(`Realm settings → User registration → Default roles`) — é o mesmo efeito que os
`oidc-hardcoded-role-idp-mapper` fazem hoje para o login social.

Com (b) você apaga as telas de registro, os services e os endpoints de criação de usuário das duas
APIs. O KShop ainda precisa criar o `UserProfile` local no primeiro acesso, já que hoje isso é feito
no fluxo de registro — sem ele, `/api/userprofiles/me` responde 404 e criar produto falha.

## Ciclo de desenvolvimento

`start-dev` não cacheia tema, então:

1. Edite o `.ftl` ou o `.css` em `sso/deploy/keycloak/themes/...`
2. F5 na tela de login

Sem restart, sem rebuild. Se por acaso não atualizar, confirme que o volume está montado:

```bash
docker exec sso-keycloak-1 ls /opt/keycloak/themes/
```

Para ver a tela sem passar pelo app, abra a URL de autorização direto — trocando o `client_id`
conforme o tema que quer testar:

```
http://localhost:8082/realms/studies/protocol/openid-connect/auth?client_id=todoforsso-frontend&redirect_uri=http://localhost:5173/callback&response_type=code&scope=openid
```

## Ordem sugerida

1. Monte o tema do TodoForSso e aponte só esse client — o KShop segue intacto, e você compara os
   dois lado a lado.
2. Gostando do resultado, replique para o `kshop`.
3. Só então remova os forms das SPAs (Passo 6) e desligue o direct grant.
4. Por último, decida o cadastro (Passo 7).

Fazer o passo 3 antes do 1 te deixa sem tela de login usável enquanto o tema não estiver pronto.
