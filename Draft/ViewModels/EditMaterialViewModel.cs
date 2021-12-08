using Draft.db;
using Draft.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Draft.ViewModels
{
    public class EditMaterialViewModel : BaseViewModel
    {
        private BitmapImage imageMaterial;
        public Material EditMaterial { get; set; }
        public EditMaterialViewModel(Material material)
        {
            if (material == null)
                EditMaterial = new Material();
            else
            {
                EditMaterial = new Material
                {
                    ID = material.ID,
                    MaterialType = material.MaterialType,
                    Title = material.Title,
                    Description = material.Description,
                    CountInPack = material.CountInPack,
                    CountInStock = material.CountInStock,
                    MinCount = material.MinCount,
                    Cost = material.Cost,
                    Unit = material.Unit,
                };
                
            }
            SelectImage = new CustomCommand(() =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    try
                    {
                        var info = new FileInfo(ofd.FileName);
                        if (info.Length > 2 * 1024 * 1024)
                        {
                            MessageBox.Show("Размер фото не должен превышать 2МБ");
                            return;
                        }
                        ImageMaterial = GetImageFromPath(ofd.FileName);
                        EditMaterial.Image = $"/materials/{info.Name}";
                        var newPath = Environment.CurrentDirectory + EditMaterial.Image;
                        File.Copy(ofd.FileName, newPath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            });

            SaveMaterial = new CustomCommand(() =>
            {
                if (EditMaterial.ID == 0)
                    DBInstance.Get().Material.Add(EditMaterial);
                else
                {
                    DBInstance.Get().Entry(material).CurrentValues.SetValues(EditMaterial);
                    DBInstance.Get().SaveChanges();
                    MainWindow.Navigate(new MaterialList());
                }
            });
        }

        public BitmapImage ImageMaterial
        {
            get => imageMaterial;
            set
            {
                imageMaterial = value;
                SignalChanged();
            }
        }

        public CustomCommand SelectImage { get; set; }
        public CustomCommand SaveMaterial { get; set; }

        private BitmapImage GetImageFromPath(string url)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(url, UriKind.Absolute);
            img.EndInit();
            return img;
        }
    }
}
