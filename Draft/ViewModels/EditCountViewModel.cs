using Draft.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draft.ViewModels
{
    public class EditCountViewModel : BaseViewModel
    {
        public CustomCommand SaveCount { get; set; }

        public List<Material> Materials { get; set; }
        
        private string minCountValue;
        public string MinCountValue
        {
            get => minCountValue;
            set
            {
                minCountValue = value;
                SignalChanged();
            }
        }
        public EditCountViewModel(List<Material> materials)
        {
            var connection = DBInstance.Get();
            materials.Sort((x, y) => x.MinCount.CompareTo(y.MinCount));
            MinCountValue = materials.Last().MinCount.ToString();
            SignalChanged(MinCountValue);

            SaveCount = new CustomCommand(() =>
            {
                foreach (var materialsave in materials)
                {
                    materialsave.MinCount = int.Parse(MinCountValue);
                }
                connection.SaveChanges();
            });
        }
    }
}
