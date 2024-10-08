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
        public List<emp_info> Employees { get; set; } = new List<emp_info>();

        public emp_info SelectedEmp { get; set; } = new emp_info();

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

        public List<AssetTransfer_> AssetTransfers { get; set; } = new List<AssetTransfer_>();

        public List<emp_info> AssetTransferEmployees { get; set; } = new List<emp_info>();

        public VendorList VendorInfo { get; set; } = new VendorList();
    }

    public class AssetTransferPostModel
    {
        public string allocatedempid { get; set; }
        public string allocatedempname { get; set; }
        public string assignedbyid { get; set; }

        public string assignedbyname { get; set; }
        public string transferdate { get; set; }
        public int sno { get; set; } = 0;

        public string location { get; set; }
    }
}