<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=messagesPerField.exists('global'); section>

  <#if section = "header">
    Crie sua conta

  <#elseif section = "form">
    <form id="kc-register-form" action="${url.registrationAction}" method="post" novalidate="novalidate">

      <#--
        Os campos vêm do "user profile" do realm (profile.attributes), não são fixos.
        Esta versão cobre os tipos de texto do perfil padrão (username, email, firstName,
        lastName). Se um dia o perfil ganhar select/textarea/multivalued, este laço precisa
        crescer — o original em user-profile-commons.ftl trata todos os casos.
      -->
      <#list profile.attributes as attribute>
        <div class="field">
          <label for="${attribute.name}">
            ${advancedMsg(attribute.displayName!attribute.name)}
            <#if attribute.required><span class="required">*</span></#if>
          </label>
          <input id="${attribute.name}" name="${attribute.name}"
                 type="<#if attribute.name == 'email'>email<#else>text</#if>"
                 value="${(attribute.value!'')}"
                 <#if attribute.autocomplete??>autocomplete="${attribute.autocomplete}"</#if>
                 <#if attribute.readOnly>disabled</#if>
                 aria-invalid="${messagesPerField.existsError('${attribute.name}')?c}" />
          <#if messagesPerField.existsError('${attribute.name}')>
            <span class="field-error">${kcSanitize(messagesPerField.get('${attribute.name}'))?no_esc}</span>
          </#if>
        </div>

        <#-- Senha aparece logo depois do campo que serve de login, como no template original -->
        <#if passwordRequired?? && (attribute.name == 'username' || (attribute.name == 'email' && realm.registrationEmailAsUsername))>
          <div class="field">
            <label for="password">Senha <span class="required">*</span></label>
            <input id="password" name="password" type="password" autocomplete="new-password"
                   aria-invalid="${messagesPerField.existsError('password')?c}" />
            <#if messagesPerField.existsError('password')>
              <span class="field-error">${kcSanitize(messagesPerField.get('password'))?no_esc}</span>
            </#if>
          </div>
          <div class="field">
            <label for="password-confirm">Confirme a senha <span class="required">*</span></label>
            <input id="password-confirm" name="password-confirm" type="password" autocomplete="new-password"
                   aria-invalid="${messagesPerField.existsError('password-confirm')?c}" />
            <#if messagesPerField.existsError('password-confirm')>
              <span class="field-error">${kcSanitize(messagesPerField.get('password-confirm'))?no_esc}</span>
            </#if>
          </div>
        </#if>
      </#list>

      <#if recaptchaRequired?? && (recaptchaVisible!false)>
        <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}" data-action="${recaptchaAction}"></div>
      </#if>

      <button class="btn btn-primary" type="submit">Criar conta</button>
      <a class="link-muted" href="${url.loginUrl}">${kcSanitize(msg("backToLogin"))?no_esc}</a>
    </form>
  </#if>

</@layout.registrationLayout>
