using Dapper;
using System;
using System.Data;

namespace DbNetSuiteCoreSamples.Models
{
    public class DecimalTypeHandler : SqlMapper.TypeHandler<decimal>
    {
        public override void SetValue(IDbDataParameter parameter, decimal value)
        {
            parameter.Value = value;
        }
        public override decimal Parse(object value)
        {
            return Convert.ToDecimal(value);
        }
    }
}
