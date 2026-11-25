using System;
using System.Data;

namespace TecnoStore.Data
{
    /// <summary>Consultas del Dashboard (solo lectura, sobre vistas y funciones existentes).</summary>
    public static class DashboardRepository
    {
        /// <summary>Los números del Inicio (sin costos: los puede leer cualquier rol).</summary>
        public static DataRow Inicio()
        {
            return Db.Consulta(@"
                DECLARE @ini DATE = DATEADD(DAY, -90, CAST(GETDATE() AS DATE));
                SELECT
                  (SELECT COUNT(*) FROM Venta WHERE estado = 'Completada' AND fecha >= @ini) AS ventas,
                  (SELECT ISNULL(SUM(total), 0) FROM Venta WHERE estado = 'Completada' AND fecha >= @ini) AS ingresos,
                  (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE es_final = 0) AS ordenes_abiertas,
                  (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE estado = 'Lista') AS ordenes_listas,
                  (SELECT COUNT(*) FROM vw_StockBajo) AS alertas_stock").Rows[0];
        }

        /// <summary>Indicadores de los últimos 90 días comparados con los 90 anteriores.</summary>
        public static DataRow Indicadores()
        {
            return Db.Consulta(@"
                -- Periodo: últimos 90 días; se compara con los 90 días anteriores
                DECLARE @ini DATE = DATEADD(DAY, -90, CAST(GETDATE() AS DATE));
                DECLARE @ant DATE = DATEADD(DAY, -90, @ini);
                SELECT
                  (SELECT COUNT(*) FROM Venta WHERE estado = 'Completada' AND fecha >= @ini) AS ventas_mes,
                  (SELECT COUNT(*) FROM Venta WHERE estado = 'Completada' AND fecha >= @ant AND fecha < @ini) AS ventas_ant,
                  (SELECT ISNULL(SUM(total), 0) FROM Venta WHERE estado = 'Completada' AND fecha >= @ini) AS ingresos_mes,
                  (SELECT ISNULL(SUM(total), 0) FROM Venta WHERE estado = 'Completada' AND fecha >= @ant AND fecha < @ini) AS ingresos_ant,
                  (SELECT ISNULL(SUM(dv.cantidad), 0) FROM DetalleVenta dv JOIN Venta v ON v.id_venta = dv.id_venta
                    WHERE v.estado = 'Completada' AND v.fecha >= @ini) AS unidades_mes,
                  (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE es_final = 0) AS ordenes_abiertas,
                  (SELECT COUNT(*) FROM vw_OrdenServicioCompleta WHERE estado = 'Lista') AS ordenes_listas,
                  (SELECT COUNT(*) FROM vw_StockBajo) AS alertas_stock,
                  (SELECT COUNT(*) FROM vw_StockBajo WHERE stock_actual = 0) AS agotados,
                  (SELECT COUNT(*) FROM Cliente WHERE activo = 1) AS clientes_activos,
                  (SELECT COUNT(*) FROM Cliente WHERE activo = 1 AND fecha_registro >= @ini) AS clientes_nuevos,
                  (SELECT COUNT(*) FROM Garantia WHERE fecha_fin >= CAST(GETDATE() AS DATE)) AS garantias_vigentes,
                  (SELECT ISNULL(SUM(stock_actual * costo), 0) FROM Producto WHERE activo = 1) AS valor_inventario").Rows[0];
        }

        /// <summary>Ingresos de los últimos 6 meses (vista vw_ResumenMensual).</summary>
        public static DataTable IngresosPorMes()
        {
            return Db.Consulta(@"
                SELECT mes, ventas, ingresos FROM vw_ResumenMensual
                WHERE mes >= FORMAT(DATEADD(MONTH, -5, GETDATE()), 'yyyy-MM') ORDER BY mes");
        }

        /// <summary>Ingresos por categoría de los últimos 90 días (función fn_VentasPorCategoria).</summary>
        public static DataTable VentasPorCategoria()
        {
            return Db.Consulta(@"SELECT categoria, ingresos FROM dbo.fn_VentasPorCategoria(DATEADD(DAY, -90, GETDATE()), GETDATE())
                                 ORDER BY ingresos DESC");
        }

        public static DataTable AlertasStock()
        {
            return Db.Consulta(@"SELECT TOP 8 sku, nombre AS producto, stock_actual AS stock, stock_minimo AS minimo
                                 FROM vw_StockBajo ORDER BY faltante DESC");
        }

        public static DataTable OrdenesAbiertas()
        {
            return Db.Consulta(@"SELECT TOP 8 folio, equipo_descripcion AS equipo, tecnico, estado, dias_en_taller AS dias
                                 FROM vw_OrdenServicioCompleta WHERE es_final = 0 ORDER BY dias_en_taller DESC");
        }
    }
}
