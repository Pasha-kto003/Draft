using Draft.db;
using Draft.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draft.ViewModels
{
    public class MaterialListViewModel : BaseViewModel
    {
        private List<Material> materials;
        public List<Material> Materials
        {
            get => materials;
            set
            {
                materials = value;
                SignalChanged();
            }
        }
        private List<MaterialType> materialTypes;
        public List<MaterialType> MaterialTypes
        {
            get => materialTypes;
            set
            {
                materialTypes = value;
                SignalChanged();
            }
        }
        private List<Supplier> suppliers;
        public List<Supplier> Suppliers
        {
            get => suppliers;
            set
            {
                suppliers = value;
                SignalChanged();
            }
        }

        private string supplierString = "453545";
        public string SuppliersString
        {
            get => supplierString;
            set
            {
                supplierString = value;
                SignalChanged();
            }
        }
        public List<string> ViewCountRows { get; set; }
        public string SelectedViewCountRows
        {
            get => selectedViewCountRows;
            set
            {
                selectedViewCountRows = value;
                paginationPageIndex = 0;
                Pagination();
            }
        }
        public string SearchCountRows
        {
            get => searchCountRows;
            set
            {
                searchCountRows = value;
                SignalChanged();
            }
        }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Search();
            }
        }

        private MaterialType selectedTypeFilter;
        public List<MaterialType> TypeFilter { get; set; }
        public MaterialType SelectedTypeFilter
        {
            get => selectedTypeFilter;
            set
            {
                selectedTypeFilter = value;
                Search();
            }
        }

        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                Search();
            }
        }

        public Material SelectedMaterial { get; set; }

        

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        public CustomCommand AddMaterial { get; set; }
        public CustomCommand EditMaterial { get; set; }
        public CustomCommand RemoveMaterial { get; set; }


        List<Material> searchResult;
        int paginationPageIndex = 0;
        private string searchCountRows;
        private string selectedViewCountRows;
        public int rows = 0;
        public int CountPages = 0;

        private string pages;
        public string Pages
        {
            get => pages;
            set
            {
                pages = value;
                SignalChanged();
            }
        }
        public MaterialListViewModel()
        {
            var connection = DBInstance.Get();

            Materials = new List<Material>(connection.Material.ToList());
            MaterialTypes = new List<MaterialType>(connection.MaterialType.ToList());
            Suppliers = new List<Supplier>(connection.Supplier.ToList());

            ViewCountRows = new List<string>();
            ViewCountRows.AddRange(new string[] { "15", "все" });
            selectedViewCountRows = ViewCountRows.First();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование", "Описание" });
            selectedSearchType = SearchType.First();

            TypeFilter = DBInstance.Get().MaterialType.ToList();
            TypeFilter.Add(new MaterialType { Title = "все"});
            selectedTypeFilter = TypeFilter.Last();

            BackPage = new CustomCommand(() => {
                if (searchResult == null)
                    return;
                if (paginationPageIndex > 0)
                    paginationPageIndex--;
                Pagination();
            });

            ForwardPage = new CustomCommand(() =>
            {
                if (searchResult == null)
                    return;
                int.TryParse(SelectedViewCountRows, out int rowsOnPage);
                if (rowsOnPage == 0)
                    return;
                int countPage = searchResult.Count() / rowsOnPage;
                if (searchResult.Count() % rowsOnPage != 0)
                    countPage++;
                if (countPage > paginationPageIndex + 1)
                    paginationPageIndex++;
                Pagination();

            });

            AddMaterial = new CustomCommand(() =>
            {
                MainWindow.Navigate(new EditMaterialView());
            });
            EditMaterial = new CustomCommand(() =>
            {
                if (SelectedMaterial == null)
                    return;
                MainWindow.Navigate(new EditMaterialView(SelectedMaterial));
            });
            searchResult = DBInstance.Get().Material.ToList();
            InitPagination();
            Pagination();
            Search();
        }
        private void InitPagination()
        {
            SearchCountRows = $"Найдено записей: {searchResult.Count} из {DBInstance.Get().Material.Count()}";
            paginationPageIndex = 0;
        }

        private void Pagination()
        {
            int rowsOnPage = 0;
            if (!int.TryParse(SelectedViewCountRows, out rowsOnPage))
            {
                Materials = searchResult;
            }
            else
            {
                Materials = searchResult.Skip(rowsOnPage * paginationPageIndex)
                    .Take(rowsOnPage).ToList();
                SignalChanged("Materials");
                int.TryParse(SelectedViewCountRows, out rows);
                CountPages = searchResult.Count() / rows;
                Pages = $"{paginationPageIndex + 1}/{CountPages + 1}";
            }
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            if (SelectedSearchType == "Наименование")
                searchResult = DBInstance.Get().Material
                    .Where(c =>c.Title.ToLower().Contains(search)).ToList();
            else if (SelectedSearchType == "Описание")
                searchResult = DBInstance.Get().Material
                    .Where(c =>c.Description.ToLower().Contains(search)).ToList();
            InitPagination();
            Pagination();
        }
    }
}
