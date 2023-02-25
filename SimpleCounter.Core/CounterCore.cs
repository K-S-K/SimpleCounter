using SimpleCounter.Data;
using SimpleCounter.Draw;

namespace SimpleCounter.Core
{
    public class CounterCore : ICounterCore
    {
        private ICounterDraw _counterDraw;
        private ICounterData _counterData;

        public CounterCore(ICounterData counterData, ICounterDraw counterDraw)
        {
            _counterData = counterData;
            _counterDraw = counterDraw;
        }

        public string CreateCounterImage(Guid pageId)
        {
            int count = _counterData.GetCounterValue(pageId);

            string svgContent = _counterDraw.CreateCounterImage(count);

            return svgContent;
        }
    }
}