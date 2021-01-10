using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClothesLotModel;
using System.Data.Entity;
using System.Data;






namespace Rad_Alexandra_Proiect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        ClothesLotEntitiesModel ctx = new ClothesLotEntitiesModel();
        CollectionViewSource sellerViewSource;

        Binding firstNameTextBoxBinding = new Binding();
        Binding lastNameTextBoxBinding = new Binding();

        Binding colorTextBoxBinding = new Binding();
        Binding categoryTextBoxBinding = new Binding();

        Binding cmbSellersBinding = new Binding();
        Binding cmbProductBinding = new Binding();

        CollectionViewSource productViewSource;
        CollectionViewSource sellerReturnsViewSource;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            firstNameTextBoxBinding.Path = new PropertyPath("FirstName");
            lastNameTextBoxBinding.Path = new PropertyPath("LastName");
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);


            colorTextBoxBinding.Path = new PropertyPath("Color");
            categoryTextBoxBinding.Path = new PropertyPath("Category");
            colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
            categoryTextBox.SetBinding(TextBox.TextProperty, categoryTextBoxBinding);

            cmbSellersBinding.Path = new PropertyPath("SellerId");
            cmbProductBinding.Path = new PropertyPath("ProdId");
            cmbSellers.SetBinding(TextBox.TextProperty, cmbSellersBinding);
            cmbProduct.SetBinding(TextBox.TextProperty, cmbProductBinding);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {



            sellerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sellerViewSource")));
            sellerViewSource.Source = ctx.Sellers.Local;

            sellerReturnsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("sellerReturnsViewSource")));
            sellerReturnsViewSource.Source = ctx.Returns.Local;


            productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.Source = ctx.Products.Local;

            // sellerReturnsViewSource.Source = ctx.Returns.Local;

            ctx.Sellers.Load();
            ctx.Returns.Load();
            ctx.Products.Load();

            cmbSellers.ItemsSource = ctx.Sellers.Local;
            cmbSellers.SelectedValuePath = "SellerId";
            //cmbSellers.DisplayMemberPath = "FirstName";



            cmbProduct.SelectedValuePath = "ProdId";
            cmbProduct.ItemsSource = ctx.Products.Local;

            //cmbProduct.DisplayMemberPath = "Category";

            BindDataGrid();
        }


        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;

            sellerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            sellerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
            ////firstNameTextBox.Text = tempfirst;
            ////lastNameTextBox.Text = templast;
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            sellerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            sellerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            sellerViewSource.View.MoveCurrentToNext();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Seller seller = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    seller = new Seller()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    ctx.Sellers.Add(seller);
                    sellerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnDelete.IsEnabled = true;
                sellerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
            else
            if (action == ActionState.Edit)
            {
                try
                {
                    seller = (Seller)sellerDataGrid.SelectedItem;
                    seller.FirstName = firstNameTextBox.Text.Trim();
                    seller.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                sellerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                sellerViewSource.View.MoveCurrentTo(seller);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                sellerDataGrid.IsEnabled = false;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
            }
            else
            if (action == ActionState.Delete)
            {
                try
                {
                    seller = (Seller)sellerDataGrid.SelectedItem;
                    ctx.Sellers.Remove(seller);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                sellerViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                sellerDataGrid.IsEnabled = false;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
                lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding) ;
            }

        }

        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;

            productDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            categoryTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(categoryTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            categoryTextBox.Text = "";
            colorTextBox.Text = "";
            Keyboard.Focus(colorTextBox);
        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            categoryTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(categoryTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            SetValidationBinding();
            ////firstNameTextBox.Text = tempfirst;
            ////lastNameTextBox.Text = templast;
            Keyboard.Focus(colorTextBox);
        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            BindingOperations.ClearBinding(categoryTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);

        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToNext();
        }

        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;

            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            productDataGrid.IsEnabled = true;

            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;
            colorTextBox.IsEnabled = false;
            categoryTextBox.IsEnabled = false;
        }

        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Product product = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    product = new Product()
                    {
                        Color = colorTextBox.Text.Trim(),
                        Category = categoryTextBox.Text.Trim()
                    };
                    ctx.Products.Add(product);
                    productViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnDelete1.IsEnabled = true;
                productDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                categoryTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                categoryTextBox.SetBinding(TextBox.TextProperty, categoryTextBoxBinding);
            }
            else
            if (action == ActionState.Edit)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    product.Color = colorTextBox.Text.Trim();
                    product.Category = categoryTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                productViewSource.View.MoveCurrentTo(product);

                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                productDataGrid.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                categoryTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                categoryTextBox.SetBinding(TextBox.TextProperty, categoryTextBoxBinding);
            }
            else
            if (action == ActionState.Delete)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    ctx.Products.Remove(product);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                productDataGrid.IsEnabled = false;
                colorTextBox.IsEnabled = false;
                categoryTextBox.IsEnabled = false;
                colorTextBox.SetBinding(TextBox.TextProperty, colorTextBoxBinding);
                categoryTextBox.SetBinding(TextBox.TextProperty, categoryTextBoxBinding);
            }
        }

        private void BindDataGrid()
        {
            var queryReturn = from ret in ctx.Returns
                              join sell in ctx.Sellers on ret.SellerId equals
                              sell.SellerId
                              join prod in ctx.Products on ret.ProdId
                  equals prod.ProdId
                              select new
                              {
                                  ret.ProdId,
                                  ret.ReturnsId,
                                  ret.SellerId,
                                  sell.FirstName,
                                  sell.LastName,
                                  prod.Category,
                                  prod.Color
                              };
            sellerReturnsViewSource.Source = queryReturn.ToList();
        }
        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = sellerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = sellerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, 
            lastNameValidationBinding); //setare binding nou
        }
       
        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbSellers.IsEnabled = true;
            cmbProduct.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            Keyboard.Focus(firstNameTextBox);
        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbSellers.IsEnabled = true;
            cmbProduct.IsEnabled = true;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempFirstName = firstNameTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;
            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNameTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
        }

        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;
            cmbSellers.IsEnabled = false;
            cmbProduct.IsEnabled = false;
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameTextBoxBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameTextBoxBinding);
        }

        private void btnPrev2_Click(object sender, RoutedEventArgs e)
        {
            sellerReturnsViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
           sellerReturnsViewSource.View.MoveCurrentToNext();
        }

        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Return returns = null;
            if (action == ActionState.New)
            {
                try
                {
                    Seller seller = (Seller)cmbSellers.SelectedItem;
                    Product product = (Product)cmbProduct.SelectedItem;
                    //instantiem Order entity
                    returns = new Return()
                    {

                        SellerId = seller.SellerId,
                        ProdId = product.ProdId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Returns.Add(returns);
                    sellerReturnsViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedReturn = returnsDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedReturn.ReturnsId;
                    var editedReturn = ctx.Returns.FirstOrDefault(s => s.ReturnsId == curr_id);
                    if (editedReturn != null)
                    {
                        editedReturn.ReturnsId = Int32.Parse(cmbSellers.SelectedValue.ToString());
                        editedReturn.ProdId = Convert.ToInt32(cmbProduct.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                sellerViewSource.View.MoveCurrentTo(selectedReturn);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedReturn = returnsDataGrid.SelectedItem;
                    int curr_id = selectedReturn.ReturnsId;
                    var deletedReturn = ctx.Returns.FirstOrDefault(s => s.ReturnsId == curr_id);
                    if (deletedReturn != null)
                    {
                        ctx.Returns.Remove(deletedReturn);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}

