// Dashboards de la pagina principal (usa Chart.js para los graficos)
const colores = ['#0f766e', '#f59e0b', '#3b82f6', '#ef4444', '#8b5cf6', '#14b8a6'];
let graficoFacturacion = null;
let graficoTipos = null;

// ----- 1. Facturacion mensual -----
function armarFiltroAnio() {
    const select = document.getElementById('filtro-anio');
    const anioActual = new Date().getFullYear();
    select.innerHTML = '<option value="">Últimos 12 meses</option>';
    for (let anio = anioActual; anio >= anioActual - 3; anio--) {
        select.innerHTML += `<option value="${anio}">Año ${anio}</option>`;
    }
    select.addEventListener('change', cargarFacturacion);
}

async function cargarFacturacion() {
    const anio = document.getElementById('filtro-anio').value;
    try {
        const datos = await api('/dashboard/facturacion-mensual' + (anio ? `?anio=${anio}` : ''));
        const total = datos.reduce((suma, d) => suma + d.total, 0);
        document.getElementById('total-facturacion').textContent = formatearMoneda(total);

        if (graficoFacturacion) graficoFacturacion.destroy();
        graficoFacturacion = new Chart(document.getElementById('grafico-facturacion'), {
            type: 'bar',
            data: {
                labels: datos.map(d => d.etiqueta),
                datasets: [{
                    label: 'Facturación',
                    data: datos.map(d => d.total),
                    backgroundColor: colores[0],
                    borderRadius: 4
                }]
            },
            options: {
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: ctx => `${formatearMoneda(ctx.raw)} (${datos[ctx.dataIndex].cantidad} atenciones)`
                        }
                    }
                },
                scales: {
                    y: { beginAtZero: true, ticks: { callback: v => '$' + v.toLocaleString('es-AR') } }
                }
            }
        });
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

// ----- 2. Cantidad e importe por tipo de atencion -----
async function cargarPorTipo() {
    const params = parametrosRango('tipo-desde', 'tipo-hasta');
    try {
        const datos = await api('/dashboard/por-tipo' + params);
        const tbody = document.getElementById('tabla-tipos');

        if (datos.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted">Sin atenciones en el período.</td></tr>';
        } else {
            const totalCantidad = datos.reduce((s, d) => s + d.cantidad, 0);
            const totalImporte = datos.reduce((s, d) => s + d.importe, 0);
            tbody.innerHTML = datos.map((d, i) => `
                <tr>
                    <td><i class="bi bi-circle-fill" style="color:${colores[i % colores.length]}"></i> ${escaparHtml(d.tipo)}</td>
                    <td class="text-end">${d.cantidad}</td>
                    <td class="text-end">${formatearMoneda(d.importe)}</td>
                </tr>`).join('') + `
                <tr class="fw-bold">
                    <td>Total</td>
                    <td class="text-end">${totalCantidad}</td>
                    <td class="text-end">${formatearMoneda(totalImporte)}</td>
                </tr>`;
        }

        if (graficoTipos) graficoTipos.destroy();
        graficoTipos = new Chart(document.getElementById('grafico-tipos'), {
            type: 'pie',
            data: {
                labels: datos.map(d => d.tipo),
                datasets: [{ data: datos.map(d => d.cantidad), backgroundColor: colores }]
            },
            options: {
                maintainAspectRatio: false,
                plugins: { legend: { position: 'right' } }
            }
        });
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

// ----- 3. Top 5 de clientes -----
async function cargarTopClientes() {
    const params = parametrosRango('top-desde', 'top-hasta');
    try {
        const datos = await api('/dashboard/top-clientes' + params);
        const tbody = document.getElementById('tabla-top');

        if (datos.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Sin atenciones en el período.</td></tr>';
            return;
        }
        tbody.innerHTML = datos.map((d, i) => `
            <tr>
                <td><span class="ranking">${i + 1}</span></td>
                <td>${escaparHtml(d.cliente)}<br><small class="text-muted">${escaparHtml(d.codigo)}</small></td>
                <td class="text-end">${d.cantidad}</td>
                <td class="text-end">${formatearMoneda(d.importe)}</td>
            </tr>`).join('');
    } catch (error) {
        mostrarAlerta(error.message, 'danger');
    }
}

function parametrosRango(idDesde, idHasta) {
    const params = new URLSearchParams();
    const desde = document.getElementById(idDesde).value;
    const hasta = document.getElementById(idHasta).value;
    if (desde) params.append('desde', desde);
    if (hasta) params.append('hasta', hasta);
    const texto = params.toString();
    return texto ? '?' + texto : '';
}

document.getElementById('form-tipo').addEventListener('submit', (e) => {
    e.preventDefault();
    cargarPorTipo();
});
document.getElementById('form-top').addEventListener('submit', (e) => {
    e.preventDefault();
    cargarTopClientes();
});

armarFiltroAnio();
cargarFacturacion();
cargarPorTipo();
cargarTopClientes();
