using System;

namespace api.Mapping
{
    public interface IObjectMapper
    {
        // Crea una nueva instancia de TDest y copia propiedades desde source.
        // Puedes ignorar propiedades de destino por nombre (p. ej., "IdEmployee").
        TDest Map<TSource, TDest>(TSource source, params string[] ignoreDestProps)
            where TDest : new();

        // Copia propiedades desde source hacia una instancia existente (dest).
        // ignoreNulls=true -> no sobrescribe con null (útil para PATCH).
        // ignoreDestProps -> propiedades del destino que NO deben tocarse (p. ej., PKs).
        void MapInto<TSource, TDest>(TSource source, TDest dest, bool ignoreNulls = false, params string[] ignoreDestProps);
    }
}
