using System.Threading;

namespace Libraries.DNV.Utility
{
    public class CancellationTokenHelper
    {
        private readonly CancellationTokenSource _source;

        public CancellationToken Value => _source.Token;

        private CancellationTokenHelper(CancellationTokenSource source)
        {
            _source = source;
        }
        
        public void Cancel()
        {
            _source.Cancel();
            _source.Dispose();
        }

        public static CancellationTokenHelper Create()
        {
            return new CancellationTokenHelper(new CancellationTokenSource());
        }
    }
}