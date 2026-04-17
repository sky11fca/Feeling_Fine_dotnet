window.renderPieChart = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;

    // Destroy existing chart instance if it exists to prevent overlap
    const existingChart = Chart.getChart(canvasId);
    if (existingChart) {
        existingChart.destroy();
    }

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    'rgba(127, 175, 255, 0.7)',
                    'rgba(104, 211, 255, 0.7)',
                    'rgba(172, 170, 173, 0.7)',
                    'rgba(100, 161, 255, 0.7)',
                    'rgba(255, 255, 255, 0.5)'
                ],
                borderColor: '#19191c',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        color: '#ffffff',
                        font: {
                            family: 'Inter'
                        }
                    }
                }
            }
        }
    });
};
