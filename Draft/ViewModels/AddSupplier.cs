using Draft.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draft.ViewModels
{
    public class AddSupplier : BaseViewModel
    {
        private List<Supplier> supplier;
        public List<Supplier> Suplier
        {
            get => supplier;
            set
            {
                supplier = value;
                SignalChanged();
            }

        }
        public List<Supplier> Suppliers;
        public CustomCommand Save { get; set; }
        public Supplier AddSupplierVM { get; set; }
        public AddSupplier(Supplier supplier)
        {
            var connection = DBInstance.Get();
            Suplier = connection.Supplier.ToList();

            if(supplier == null)
            {
                AddSupplierVM = new Supplier { QualityRating = 100, SupplierType = "ООО" };
            }
            else
            {
                AddSupplierVM = new Supplier
                {
                    ID = supplier.ID,
                    Title = supplier.Title,
                    INN = supplier.INN,
                    QualityRating = supplier.QualityRating,
                    StartDate = supplier.StartDate,
                    SupplierType = supplier.SupplierType
                };
            }

            Save = new CustomCommand(() => 
            { 
                if(AddSupplierVM.ID == 0)
                {
                    connection.Supplier.Add(AddSupplierVM);
                }
                else
                {
                    connection.Entry(supplier).CurrentValues.SetValues(AddSupplierVM);
                }
                connection.SaveChanges();
                SignalChanged("Supplier");
            });
        }
    }
}
