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
    <h2 class="brand">KShop</h2>

    <div class="card">
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
