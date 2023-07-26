using System.Threading.Tasks;

namespace FaxCap.Common.Abstract
{
    public interface ICompletable
    {
        Task Complete(bool isSuccessful = true);
    }
}
