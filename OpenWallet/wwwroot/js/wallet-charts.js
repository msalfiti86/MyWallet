(() => {
  const ctx = document.getElementById("monthlySpendingChart");
  if (!ctx || typeof Chart === "undefined") return;

  new Chart(ctx, {
    type: "bar",
    data: {
      labels: ["Jan", "Feb", "Mar", "Apr", "May", "Jun"],
      datasets: [
        { label: "Credit", data: [12000, 18000, 15000, 26000, 21000, 31000], backgroundColor: "#14B8A6", borderRadius: 8 },
        { label: "Debit", data: [8000, 9800, 12400, 7400, 11200, 9400], backgroundColor: "#F59E0B", borderRadius: 8 }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { position: "bottom" } },
      scales: { x: { grid: { display: false } }, y: { grid: { color: "rgba(148,163,184,.18)" } } }
    }
  });
})();
