<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username','password') displayInfo=true; section>

  <#if section = "header">
    Entrar

  <#elseif section = "form">
    <#if realm.password>
      <form id="kc-form-login" action="${url.loginAction}" method="post" novalidate="novalidate"
            onsubmit="login.disabled = true; return true;">
        <div class="field">
          <label for="username">
            <#if !realm.loginWithEmailAllowed>Usuário<#elseif !realm.registrationEmailAsUsername>Usuário ou e-mail<#else>E-mail</#if>
          </label>
          <input id="username" name="username" type="text" autofocus autocomplete="username"
                 value="${login.username!''}"
                 aria-invalid="${messagesPerField.existsError('username','password')?c}" />
        </div>

        <div class="field">
          <label for="password">Senha</label>
          <input id="password" name="password" type="password" autocomplete="current-password"
                 aria-invalid="${messagesPerField.existsError('username','password')?c}" />
        </div>

        <#if messagesPerField.existsError('username','password')>
          <span class="field-error" aria-live="polite">
            ${kcSanitize(messagesPerField.getFirstError('username','password'))?no_esc}
          </span>
        </#if>

        <#if realm.rememberMe && !usernameHidden??>
          <label class="checkbox">
            <input type="checkbox" name="rememberMe" <#if login.rememberMe??>checked</#if> />
            <span>${msg("rememberMe")}</span>
          </label>
        </#if>

        <input type="hidden" id="id-hidden-input" name="credentialId"
               <#if auth.selectedCredential?has_content>value="${auth.selectedCredential}"</#if> />

        <button class="btn btn-primary" name="login" id="kc-login" type="submit">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
            <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4M10 17l5-5-5-5M15 12H3"/>
          </svg>
          Entrar
        </button>

        <#if realm.resetPasswordAllowed>
          <a class="link-muted" href="${url.loginResetCredentialsUrl}">${msg("doForgotPassword")}</a>
        </#if>
      </form>
    </#if>

  <#elseif section = "socialProviders">
    <#if realm.password && social.providers?? && social.providers?has_content>
      <div class="divider"><span>OU</span></div>
      <div class="social-list">
        <#list social.providers as p>
          <a id="social-${p.alias}" class="btn btn-social" href="${p.loginUrl}">
            <#if p.alias = "google">
              <svg viewBox="0 0 488 512" fill="currentColor" aria-hidden="true">
                <path d="M488 261.8C488 403.3 391.1 504 248 504 110.8 504 0 393.2 0 256S110.8 8 248 8c66.8 0 123 24.5 166.3 64.9l-67.5 64.9C258.5 52.6 94.3 116.6 94.3 256c0 86.5 69.1 156.6 153.7 156.6 98.2 0 135-70.4 140.8-106.9H248v-85.3h236.1c2.3 12.7 3.9 24.9 3.9 41.4z"/>
              </svg>
            </#if>
            Entrar com ${p.displayName!p.alias}
          </a>
        </#list>
      </div>
    </#if>

  <#elseif section = "info">
    <#-- O cadastro deste projeto é feito pela API da aplicação, não pelo Keycloak.
         Se um dia `User registration` for ativado no realm, o link nativo assume sozinho. -->
    <#if realm.registrationAllowed && !registrationDisabled??>
      <a href="${url.registrationUrl}">Crie sua conta</a>
    <#else>
      <a href="http://localhost:5174/register">Crie sua conta</a>
    </#if>
  </#if>

</@layout.registrationLayout>
