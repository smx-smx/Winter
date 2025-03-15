using Smx.Winter.Cbs.Native;

namespace Smx.Winter.Gui.Models
{
    public class CbsPackageViewModel
    {
        private readonly ICbsPackage _package;
        private readonly Lazy<string> _getPackageId;

        public string StringId => _getPackageId.Value;

        public CbsPackageViewModel(ICbsPackage package)
        {
            _package = package;
            _getPackageId = new Lazy<string>(() => _package.GetIdentity().GetStringId());
        }
    }
}
