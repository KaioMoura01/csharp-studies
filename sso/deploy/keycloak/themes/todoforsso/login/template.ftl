<#--
  Casca compartilhada por todas as páginas do fluxo de login deste tema.
  As páginas (login.ftl e afins) preenchem as sections via <#nested "nome">.
-->
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

<body class="${bodyClass}">
  <div class="page">
    <div class="card">
      <div class="brand">
        <svg class="brand-icon" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
          <path d="M12 2l1.9 5.6L19.5 9.5l-5.6 1.9L12 17l-1.9-5.6L4.5 9.5l5.6-1.9L12 2z"/>
          <path d="M18.5 14l.9 2.6 2.6.9-2.6.9-.9 2.6-.9-2.6-2.6-.9 2.6-.9.9-2.6z" opacity=".7"/>
        </svg>
        <span class="brand-name">TodoForSso</span>
      </div>

      <h1 class="card-title"><#nested "header"></h1>

      <#if displayMessage && message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
        <div class="alert alert-${message.type}">
          ${kcSanitize(message.summary)?no_esc}
        </div>
      </#if>

      <#nested "form">

      <#if auth?has_content && auth.showTryAnotherWayLink()>
        <form id="kc-select-try-another-way-form" action="${url.loginAction}" method="post">
          <input type="hidden" name="tryAnotherWay" value="on"/>
          <button class="btn btn-ghost" type="submit">${kcSanitize(msg("doTryAnotherWay"))?no_esc}</button>
        </form>
      </#if>

      <#nested "socialProviders">

      <#if displayInfo>
        <div class="card-footer"><#nested "info"></div>
      </#if>
    </div>
  </div>
</body>
</html>
</#macro>
