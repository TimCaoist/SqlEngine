using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tim.SqlEngine.Models;

namespace Tim.SqlEngine.SqlHelper.QueryHandler.ReleatedFillHandler
{
    public static class ReleatedFillHandlerFactory
    {
        public static IFillHandler Create(ReleatedQuery config)
        {
            switch (config.FillType)
            {
                case 0:
                    return ArrayFillHandler.Instance;
                case 1:
                    return SingleRecodFillHandler.Instance;
                case 2:
                    return FieldsFillHandler.Instance;
            }

            throw new ArgumentException("未找到匹配的字段填充方式!");
        }
    }
}
