using Draft.db;
using Draft.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        private MaterialType selectedMaterialTypeFilter;
        public List<MaterialType> MaterialTypeFilter { get; set; }
        public MaterialType SelectedMaterialTypeFilter
        {
            get => selectedMaterialTypeFilter;
            set
            {
                selectedMaterialTypeFilter = value;
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

        public List<string> SortType { get; set; }
        private string selectedSortType;
        public string SelectedSortType
        {
            get => selectedSortType;

            set
            {
                selectedSortType = value;
                Sort();
            }
        }

        private string stringSupplier;

        public string SrtingSupplier
        {
            get => stringSupplier;

            set 
            {
                stringSupplier = value;
                SignalChanged();
            }
        }

        private string colorForXaml;

        public string ColorForXaml
        {
            get => colorForXaml;

            set
            {
                colorForXaml = value;
                SignalChanged();
            }
        }

        public Material SelectedMaterial { get; set; }

        

        public CustomCommand BackPage { get; set; }
        public CustomCommand ForwardPage { get; set; }
        public CustomCommand AddMaterial { get; set; }
        public CustomCommand EditMaterial { get; set; }
        public CustomCommand RemoveMaterial { get; set; }
        public CustomCommand EditMinCount { get; set; }
        public CustomCommand WriteIn { get; set; }

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

        public List<Material> SelectedMaterials { get; set; }

        private string selectedOrderType;
        public List<string> OrderType { get; set; }
        public string SelectedOrderType
        {
            get => selectedOrderType;
            set
            {
                selectedOrderType = value;
                Sort();
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

            MaterialTypeFilter = DBInstance.Get().MaterialType.ToList();
            MaterialTypeFilter.Add(new MaterialType { Title = "Все типы" });
            selectedMaterialTypeFilter = MaterialTypeFilter.Last();

            SortType = new List<string>();
            SortType.AddRange(new string[] { "Наименование", "Отмена", "Стоимость", "Остаток" });
            selectedSortType = SortType.First();

            OrderType = new List<string>();
            OrderType.AddRange(new string[] { "По возрастанию", "По убыванию", "По умолчанию" });
            selectedOrderType = OrderType.Last();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Наименование", "Описание" });
            selectedSearchType = SearchType.First();

            foreach (var material in Materials)
            {
                if(material.CountInStock < material.MinCount)
                {
                    material.ColorForXaml = "#f19292";
                }

                else if(material.CountInStock > material.MinCount * 3)
                {
                    material.ColorForXaml = "#ffba01";
                }
                
                foreach (var suplier in material.Supplier)
                {               
                    if (suplier != material.Supplier.Last())
                        material.SrtingSupplier += $"{suplier.Title}, ";
                    material.SrtingSupplier += $"{suplier.Title}";
                }             
            }

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
                CountPages = countPage;
                if (searchResult.Count() % rowsOnPage != 0)
                    countPage++;
                if (countPage > paginationPageIndex + 1)
                    paginationPageIndex++;
                Pagination();

            });

            EditMinCount = new CustomCommand(() =>
            {
                EditCount modalWindow = new EditCount(SelectedMaterials);
                modalWindow.ShowDialog();
            });

            AddMaterial = new CustomCommand(() =>
            {
                AddMaterialView addMaterial = new AddMaterialView();
                addMaterial.ShowDialog();
                LoadEntities();
                InitPagination();
                Pagination();
            });


            EditMaterial = new CustomCommand(() =>
            {
                if (SelectedMaterial == null) return;
                AddMaterialView addMaterial = new AddMaterialView(SelectedMaterial);
                addMaterial.ShowDialog();
                LoadEntities();
                InitPagination();
                Pagination();
            });

            searchResult = DBInstance.Get().Material.ToList();
            InitPagination();
            Pagination();

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
                Materials = searchResult.Skip(rowsOnPage * paginationPageIndex).Take(rowsOnPage).ToList();
                SignalChanged("Materials");
                int.TryParse(SelectedViewCountRows, out rows);
                CountPages = searchResult.Count() / rows;
                Pages = $"{paginationPageIndex + 1} из {CountPages + 1}";
            }
        }

        public void LoadEntities()
        {
            Materials = new List<Material>(DBInstance.Get().Material.ToList());
            MaterialTypes = new List<MaterialType>(DBInstance.Get().MaterialType.ToList());
            Suppliers = new List<Supplier>(DBInstance.Get().Supplier.ToList());
            foreach (var mat in Materials)
            {

                if (mat.CountInStock < mat.MinCount)
                {
                    mat.ColorForXaml = "#f19292";
                }
                else if (mat.CountInStock > mat.MinCount * 3)
                {
                    mat.ColorForXaml = "#ffba01";
                }
                mat.SrtingSupplier = "";
                foreach (var sup in mat.Supplier)
                {
                    if (sup != mat.Supplier.Last())
                        mat.SrtingSupplier += $"{sup.Title}, ";
                    else
                        mat.SrtingSupplier += $"{sup.Title}";
                }
            }
        }

        public MaterialListViewModel(List<Material> materials) : this()
        {
            EditCount modalWindow = new EditCount(materials);
            modalWindow.ShowDialog();
        }

        private void Search()
        {
            var search = SearchText.ToLower();
            if (SelectedMaterialTypeFilter.Title == "Все типы")
            {
                if (SelectedSearchType == "Наименование")
                    searchResult = DBInstance.Get().Material
                        .Where(c => c.Title.ToLower().Contains(search)).ToList();
                else if (SelectedSearchType == "Описание")
                    searchResult = DBInstance.Get().Material
                        .Where(c => c.Description.ToLower().Contains(search)).ToList();
            }
            else
            {
                if (SelectedSearchType == "Наименование")
                    searchResult = DBInstance.Get().Material
                        .Where(c => c.Title.ToLower().Contains(search) && c.MaterialType.Title.Contains(SelectedMaterialTypeFilter.Title)).ToList();
                else if (SelectedSearchType == "Описание")
                    searchResult = DBInstance.Get().Material
                        .Where(c => c.Description.ToLower().Contains(search) && c.MaterialType.Title.Contains(SelectedMaterialTypeFilter.Title)).ToList();
            }

            Sort();
            InitPagination();
            Pagination();
        }
        internal void Sort()
        {
            if (SelectedOrderType == "По умолчанию")
                
                return;

            if (SelectedOrderType == "По убыванию")
            {
                if (SelectedSortType == "Наименование")
                    searchResult.Sort((x, y) => y.Title.CompareTo(x.Title));
                else if (SelectedSortType == "Остаток")
                    searchResult.Sort((x, y) => ((Int32)x.CountInStock).CompareTo((Int32)x.CountInStock));
                else if (SelectedSortType == "Стоимость")
                    searchResult.Sort((x, y) => y.Cost.CompareTo(x.Cost));
            }

            if (SelectedOrderType == "По возрастанию")
            {
                if (SelectedSortType == "Наименование")
                    searchResult.Sort((x, y) => x.Title.CompareTo(y.Title));
                else if (SelectedSortType == "Остаток")
                    searchResult.Sort((x, y) => ((Int32)x.CountInStock).CompareTo((Int32)y.CountInStock));
                else if (SelectedSortType == "Стоимость")
                    searchResult.Sort((x, y) => x.Cost.CompareTo(y.Cost));
            }
            paginationPageIndex = 0;
            Pagination();
        }
    }
}
