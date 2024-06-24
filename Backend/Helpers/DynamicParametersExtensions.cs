using Dapper;

namespace Backend.Helpers;

public static class DynamicParametersExtensions
{
    /// <summary>
    /// Merges the specified object into the DynamicParameters instance.
    /// </summary>
    /// <param name="parameters">The DynamicParameters instance to merge into.</param>
    /// <param name="param">The object to merge.</param>
    /// <returns>The merged DynamicParameters instance.</returns>
    public static DynamicParameters MergeObject(this DynamicParameters parameters, object? param)
    {
        parameters.AddDynamicParams(param);
        return parameters;
    }
}