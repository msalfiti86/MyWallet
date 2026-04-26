(() => {
  let pendingForm = null;
  const modalElement = document.getElementById("otpModal");
  if (!modalElement || !window.bootstrap) return;

  const modal = new bootstrap.Modal(modalElement);
  const input = modalElement.querySelector("#otpModalCode");
  const confirmButton = modalElement.querySelector("[data-otp-confirm]");
  const message = modalElement.querySelector("[data-otp-message]");
  const error = modalElement.querySelector("[data-otp-error]");

  const isArabic = document.documentElement.lang === "ar";
  const text = {
    sending: isArabic ? "جاري إرسال رمز التحقق..." : "Sending verification code...",
    sent: (email) => isArabic
      ? `للمتابعة، أدخل رمز التحقق المرسل إلى بريدك الإلكتروني ${email}.`
      : `To continue the current action, enter the OTP sent to your email ${email}.`,
    failed: isArabic ? "تعذر إرسال رمز التحقق. حاول مرة أخرى." : "Could not send OTP. Please try again.",
    required: isArabic ? "أدخل رمز التحقق للمتابعة." : "Enter the verification code to continue."
  };

  document.querySelectorAll("form[data-requires-otp='true']").forEach((form) => {
    form.addEventListener("submit", async (event) => {
      const hidden = form.querySelector("[data-otp-hidden]");
      if (!hidden || hidden.value.trim()) return;

      event.preventDefault();
      pendingForm = form;
      input.value = "";
      error.classList.add("d-none");
      message.textContent = text.sending;

      try {
        const token = form.querySelector("input[name='__RequestVerificationToken']")?.value || "";
        const response = await fetch(form.dataset.otpSendUrl || `${window.location.pathname}?handler=SendOtp`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "RequestVerificationToken": token
          },
          body: JSON.stringify({ purpose: form.dataset.otpPurpose || "General" })
        });
        const result = await response.json();
        if (!response.ok || !result.sent) throw new Error("OTP send failed");
        message.textContent = text.sent(result.maskedDestination || "m***@e***.com");
        modal.show();
        setTimeout(() => input.focus(), 250);
      } catch {
        message.textContent = text.failed;
        modal.show();
      }
    });
  });

  confirmButton?.addEventListener("click", () => {
    if (!pendingForm) return;
    const code = input.value.trim();
    if (!code) {
      error.textContent = text.required;
      error.classList.remove("d-none");
      return;
    }

    const hidden = pendingForm.querySelector("[data-otp-hidden]");
    hidden.value = code;
    modal.hide();
    pendingForm.requestSubmit();
  });
})();
