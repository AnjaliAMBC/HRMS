using HRMS.Models;
using HRMS.Models.Admin;
using HRMS.Models.ITsupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMS.Helpers
{
    public class VendorHelper
    {
        private readonly HRMS_EntityFramework _dbContext;

        public VendorHelper(HRMS_EntityFramework context)
        {
            _dbContext = context;
        }
        public List<VendorTypeDropdown> GetVendorTypes()
        {
            var model = new List<VendorTypeDropdown>();
            try
            {
                var vendortypes = _dbContext.VendorTypes.ToList();
                model = vendortypes.Select(x => new VendorTypeDropdown
                {                  
                    VendorType = x.VendorType1
                }).ToList();

            }

            catch (Exception ex)
            {
                ErrorHelper.CaptureError(ex);
            }
            return model;
        }
    }
}