using Draft.db;
using Draft.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Draft.ViewModels
{
    public class AddMaterial : BaseViewModel
    {
       

        private BitmapImage imageMaterial;
        public BitmapImage ImageMaterial
        {
            get => imageMaterial;
            set
            {
                imageMaterial = value;
                SignalChanged();
            }
        }
        List<Supplier> searchResult;

        public int NeedToStore { get; set; }
        public string Unit { get; set; }
        public int MinCountToBuy { get; set; }
        public decimal MinCountCost { get; set; }

        public Supplier SelectedMaterialSupplier { get; set; }
        private ObservableCollection<Supplier> selectedMaterialSuppliers = new ObservableCollection<Supplier>();
        public ObservableCollection<Supplier> SelectedMaterialSuppliers
        {
            get => selectedMaterialSuppliers;
            set
            {
                selectedMaterialSuppliers = value;
                SignalChanged();
            }
        }

        public Material AddMaterialVM { get; set; }
        public int CountPart { get; set; }


        public Supplier SelectedSupplier { get; set; }
        private List<Supplier> supplier;
        public List<Supplier> Supplier
        {
            get => supplier;
            set
            {
                supplier = value;
                SignalChanged();
            }

        }

        public List<MaterialType> MaterialTypes { get; set; }
        public MaterialType SelectedMaterialType { get; set; }

        public List<ProductMaterial> ProductMaterials;
        public List<MaterialCountHistory> MaterialCountHistorys;
        public List<Supplier> Suppliers;
        public List<Material> Materials;

        public CustomCommand SelectImage { get; set; }
        public CustomCommand RemoveSupplier { get; set; }
        public CustomCommand AddSupplier { get; set; }
        public CustomCommand Save { get; set; }
        public CustomCommand RemoveMaterial { get; set; }
        public CustomCommand Searching { get; set; }
        public CustomCommand WriteIn { get; set; }
        public CustomCommand NewSupplier { get; set; }
        public CustomCommand EditSupplier { get; set; }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
            }
        }     

        public AddMaterial(Material material)
        {
            var connection = DBInstance.Get();
            Supplier = connection.Supplier.ToList();
            MaterialTypes = connection.MaterialType.ToList();

            

            if (material == null)
            {
                AddMaterialVM = new Material { Image = @"\materials\picture.png", MaterialType = MaterialTypes.First() };
            }
            else
            {

                AddMaterialVM = new Material
                {
                    ID = material.ID,
                    Title = material.Title,
                    MaterialType = material.MaterialType,
                    MinCount = material.MinCount,
                    Cost = material.Cost,
                    MaterialTypeID = material.MaterialTypeID,
                    MaterialCountHistory = material.MaterialCountHistory,
                    ProductMaterial = material.ProductMaterial,
                    CountInPack = material.CountInPack,
                    CountInStock = material.CountInStock,
                    Supplier = material.Supplier,
                    Image = material.Image,
                    Description = material.Description,
                    Unit = material.Unit,
                };

                if (material.CountInStock < material.MinCount)
                {
                    NeedToStore = (int)(material.MinCount - material.CountInStock);
                    Unit = material.Unit;
                    if(NeedToStore % material.CountInPack == 0)
                    {
                        MinCountToBuy = NeedToStore / (int)material.CountInPack;
                    }
                    else
                    {
                        MinCountToBuy = NeedToStore / (int)material.CountInPack + 1;
                    }
                    MinCountCost = (int)MinCountToBuy * material.Cost;
                }
                

                if (material.Supplier != null)
                {
                    SelectedMaterialSuppliers = new ObservableCollection<Supplier>(material.Supplier);
                }
            }

            NewSupplier = new CustomCommand(() =>
            {
                AddSupplierView window = new AddSupplierView();
                window.ShowDialog();
            });

            EditSupplier = new CustomCommand(() =>
            {
                if (SelectedMaterialSupplier == null) return;
                AddSupplierView window = new AddSupplierView(SelectedMaterialSupplier);
                window.ShowDialog();
            });

            RemoveMaterial = new CustomCommand(() =>
            {
                if (material.ID == 0)
                {
                    MessageBox.Show("Такой записи не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBoxResult result = MessageBox.Show("Вы точно желаете удалить материал?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ProductMaterials = new List<ProductMaterial>(connection.ProductMaterial);
                    foreach (ProductMaterial promat in ProductMaterials)
                    {
                        if (promat.Material == material || promat.MaterialID == material.ID)
                        {
                            MessageBox.Show("Удаление невозможно, материал используется на производстве!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    MaterialCountHistorys = new List<MaterialCountHistory>(connection.MaterialCountHistory);
                    foreach (MaterialCountHistory materialCountHistorie in MaterialCountHistorys)
                    {
                        if (materialCountHistorie.Material == material || materialCountHistorie.MaterialID == material.ID)
                        {
                            try
                            {
                                DBInstance.Get().MaterialCountHistory.Remove(materialCountHistorie);

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }

                        }
                    }
                    Suppliers = new List<Supplier>(connection.Supplier);
                    foreach (Supplier sup in Suppliers)
                    {
                        if (sup.Material == material)
                        {
                            try
                            {
                                DBInstance.Get().Supplier.Remove(sup);

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }

                        }
                    }
                    try
                    {
                        DBInstance.Get().Material.Remove(material);
                        DBInstance.Get().SaveChanges();
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message);
                    }


                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            CloseModalWindow(window);
                        }
                    }
                }
                else return;
            });

            Searching = new CustomCommand(() =>
            {
                var search = SearchText.ToLower();
                foreach (var sup in SelectedMaterialSuppliers)
                {
                    if (sup.Title.Equals(search, StringComparison.OrdinalIgnoreCase))
                    {
                        SelectedMaterialSuppliers.Add(sup);
                    }
                }
            });
            SelectedMaterialType = AddMaterialVM.MaterialType;

            string directory = Environment.CurrentDirectory;
            ImageMaterial = GetImageFromPath(directory.Substring(0, directory.Length - 10) + "\\" + AddMaterialVM.Image);
            SelectImage = new CustomCommand(() =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    try
                    {
                        var info = new FileInfo(ofd.FileName);
                        ImageMaterial = GetImageFromPath(ofd.FileName);
                        AddMaterialVM.Image = $"/materials/{info.Name}";
                        var newPath = directory.Substring(0, directory.Length - 10) + AddMaterialVM.Image;
                        File.Copy(ofd.FileName, newPath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });

            AddSupplier = new CustomCommand(() =>
            {
                if (SelectedSupplier == null)
                {
                    MessageBox.Show("Выберите поставщика!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (SelectedMaterialSuppliers.Contains(SelectedSupplier))
                {
                    MessageBox.Show("Этот поставщик уже есть!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    SelectedMaterialSuppliers.Add(SelectedSupplier);
                    SignalChanged("SelectedMaterialSuppliers");
                }

            });

            
            RemoveSupplier = new CustomCommand(() =>
            {
                if (SelectedSupplier == null)
                {
                    MessageBox.Show("Нужно выбрать поставщика из списка !", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    SelectedMaterialSuppliers.Remove(SelectedMaterialSupplier);
                    SignalChanged("SelectedMaterialSuppliers");
                }

            });

            Save = new CustomCommand(() =>
            {
                
                try
                {
                    if (AddMaterialVM.CountInPack < 0)
                    {
                        MessageBox.Show("Запрещается вводить отрицательные числа в упокавку");
                        return;
                    }
                    else if (AddMaterialVM.CountInStock < 0)
                    {
                        MessageBox.Show("Запрещается вводить отрицательные числа на склад");
                        return;
                    }
                    else if (AddMaterialVM.Cost < 0)
                    {
                        MessageBox.Show("Запрещается вводить отрицательные числа в цену материала");
                        return;
                    }
                    else if (AddMaterialVM.MinCount < 0)
                    {
                        MessageBox.Show("Запрещается вводить отрицательные числа в минимальное число материала");
                        return;
                    }
                    AddMaterialVM.Supplier = SelectedMaterialSuppliers;
                    if (AddMaterialVM.ID == 0)
                        connection.Material.Add(AddMaterialVM);
                    else
                    {
                        connection.Entry(material).CurrentValues.SetValues(AddMaterialVM);
                        material.Supplier = AddMaterialVM.Supplier;
                    }
    
                    connection.SaveChanges();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            CloseModalWindow(window);
                        }
                    }

                    SignalChanged("Material");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                };

                
            });

            searchResult = connection.Supplier.ToList();
        }

        private BitmapImage GetImageFromPath(string url)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(url, UriKind.Absolute);
            img.EndInit();
            return img;
        }

        public void CloseModalWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
