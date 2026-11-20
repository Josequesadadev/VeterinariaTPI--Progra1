let graficoFacturacion = null;
let graficoTipos = null;

// combo de años: ultimos 12 meses o un año puntual
function cargarAnios() {
    const combo = document.getElementById('filtro-anio');
    const anioActual = new Date().getFullYear();
    combo.innerHTML = '<option value="">Últimos 12 meses</option>';
    for (let anio = anioActual; anio >= anioActual - 3; anio--) {
        combo.innerHTML += '<option value="' + anio + '">' + anio + '</option>';
    }
}

// 1) facturacion mensual
async function cargarFacturacion() {
    const anio = document.getElementById('filtro-anio').value;
    let url = '/dashboard/facturacion-mensual';
    if (anio != '') url += '?anio=' + anio;

    try {
        const datos = await llamarApi(url);

        let total = 0;
        for (const d of datos) {
            total += d.total;
        }
        document.getElementById('total-facturacion').textContent = formatoPrecio(total);

        if (graficoFacturacion != null) {
            graficoFacturacion.destroy();
        }
        graficoFacturacion = new Chart(document.getElementById('grafico-facturacion'), {
            type: 'bar',
            data: {
                labels: datos.map(d => d.etiqueta),
                datasets: [{
                    label: 'Facturación ($)',
                    data: datos.map(d => d.total),
                    backgroundColor: '#0d6efd'
                }]
            },
            options: { maintainAspectRatio: false }
        });
    } catch (error) {
        alert(error.message);
    }
}

// 2) cantidad e importe por tipo de atencion
async function cargarPorTipo() {
    const url = '/dashboard/por-tipo' + armarFiltroFechas('tipo-desde', 'tipo-hasta');

    try {
        const datos = await llamarApi(url);

        const tabla = document.getElementById('tabla-tipos');
        tabla.innerHTML = '';
        for (const d of datos) {
            tabla.innerHTML += '<tr><td>' + d.tipo + '</td><td>' + d.cantidad + '</td><td>' + formatoPrecio(d.importe) + '</td></tr>';
        }
        if (datos.length == 0) {
            tabla.innerHTML = '<tr><td colspan="3">No hay datos</td></tr>';
        }

        if (graficoTipos != null) {
            graficoTipos.destroy();
        }
        graficoTipos = new Chart(document.getElementById('grafico-tipos'), {
            type: 'pie',
            data: {
                labels: datos.map(d => d.tipo),
                datasets: [{ data: datos.map(d => d.cantidad) }]
            },
            options: { maintainAspectRatio: false }
        });
    } catch (error) {
        alert(error.message);
    }
}

// 3) top 5 de clientes
async function cargarTopClientes() {
    const url = '/dashboard/top-clientes' + armarFiltroFechas('top-desde', 'top-hasta');

    try {
        const datos = await llamarApi(url);

        const tabla = document.getElementById('tabla-top');
        tabla.innerHTML = '';
        let posicion = 1;
        for (const d of datos) {
            tabla.innerHTML += '<tr><td>' + posicion + '</td><td>' + d.cliente + '</td><td>' + d.cantidad + '</td><td>' + formatoPrecio(d.importe) + '</td></tr>';
            posicion++;
        }
        if (datos.length == 0) {
            tabla.innerHTML = '<tr><td colspan="4">No hay datos</td></tr>';
        }
    } catch (error) {
        alert(error.message);
    }
}

function armarFiltroFechas(idDesde, idHasta) {
    const desde = document.getElementById(idDesde).value;
    const hasta = document.getElementById(idHasta).value;
    let filtro = '';

    if (desde != '') filtro += '?desde=' + desde;
    if (hasta != '') filtro += (filtro == '' ? '?' : '&') + 'hasta=' + hasta;

    return filtro;
}

cargarAnios();
cargarFacturacion();
cargarPorTipo();
cargarTopClientes();
