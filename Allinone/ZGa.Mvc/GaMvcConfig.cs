

using Allinone.ZGa.Mvc.Model.MapModel;

namespace Allinone.ZGa.Mvc
{
    public static class GaMvcConfig
    {
        public static IxMapBuilder CreateMapBuilder()
        {
            if (Allinone.Universal.FACTORYNAME == JetEazy.FactoryName.DONGGUAN)
            {
                return new DGMapBuilderClassV0();
            }
            // 使用北京芯力的读档模式
            return new BJMapBuilderClassV0();
        }
    }
}
