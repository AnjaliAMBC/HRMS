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

        public AssetModel AssetModel = new AssetModel();

    }

    public class AssetViewModel
    {
        public Asset Asset { get; set; } = new Asset();
    }

    public class AssetModel
    {
        public List<emp_info> Employees { get; set; } = new List<emp_info>();
        public List<VendorList> allVendors { get; set; } = new List<VendorList>();

        public Asset EditAssets { get; set; } = new Asset();

        public List<emp_info> ITEmployees { get; set; } = new List<emp_info>();

        public emp_info AllocatedEmpInfo { get; set; } = new emp_info();
    }
}