using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Models.ITsupport
{
    public class AssetListModel
    {
        public List<Asset> Assets { get; set; } = new List<Asset>();
        public int TotalAssets { get; set; } = 0;

        public int AssetsInUse { get; set; } = 0;

        public int AssetsInScrap { get; set; } = 0;

        public int HydAssets { get; set; } = 0;

        public int MaduraiAssets { get; set; } = 0;

    }

    public class AssetViewModel
    {
        public Asset Asset { get; set; } = new Asset();
    }


}