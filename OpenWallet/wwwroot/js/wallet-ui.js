(() => {
  const root = document.documentElement;
  const storedTheme = localStorage.getItem("openwallet-theme");
  const storedLang = localStorage.getItem("openwallet-lang") || "en";

  if (storedTheme) root.dataset.theme = storedTheme;
  root.lang = storedLang === "ar" ? "ar" : "en";
  root.dir = storedLang === "ar" ? "rtl" : "ltr";

  document.querySelector("[data-wallet-theme]")?.addEventListener("click", () => {
    const next = root.dataset.theme === "dark" ? "light" : "dark";
    root.dataset.theme = next;
    localStorage.setItem("openwallet-theme", next);
  });

  document.querySelector("[data-wallet-lang]")?.addEventListener("click", () => {
    const next = root.lang === "ar" ? "en" : "ar";
    root.lang = next;
    root.dir = next === "ar" ? "rtl" : "ltr";
    localStorage.setItem("openwallet-lang", next);
    document.cookie = `openwallet-lang=${next};path=/;max-age=31536000`;
    window.location.reload();
  });

  document.querySelector("[data-wallet-sidebar]")?.addEventListener("click", () => {
    document.getElementById("walletSidebar")?.classList.toggle("is-open");
  });

  document.querySelectorAll("[data-counter]").forEach((el) => {
    const raw = el.textContent || "";
    const numeric = Number(raw.replace(/[^\d.]/g, ""));
    if (!numeric) return;
    let frame = 0;
    const total = 32;
    const prefix = raw.trim().startsWith("SAR") ? "SAR " : "";
    const suffix = raw.includes("%") ? "%" : "";
    const tick = () => {
      frame += 1;
      el.textContent = prefix + Math.round((numeric * frame) / total).toLocaleString() + suffix;
      if (frame < total) requestAnimationFrame(tick);
    };
    tick();
  });
})();
