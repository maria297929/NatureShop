using NatureShopModel;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;


namespace NatureShop
{
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
        NatureShopEntitiesModel ctx = new NatureShopEntitiesModel();

        CollectionViewSource customerViewSource;
        Binding txtAdressBinding = new Binding();
        Binding txtMailBinding = new Binding();
        Binding txtPhoneBinding = new Binding();
        Binding txtFirst_NameBinding = new Binding();
        Binding txtLast_NameBinding = new Binding();

        CollectionViewSource productViewSource;
        Binding txtCategoryBinding = new Binding();
        Binding txtPriceBinding = new Binding();
        Binding txtProductNameBinding = new Binding();
        Binding txtQuantityBinding = new Binding();

        CollectionViewSource customerOrdersViewSource;
        Binding txtCustomer = new Binding();
        Binding txtProduct = new Binding();



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            txtAdressBinding.Path = new PropertyPath("Adress");
            txtMailBinding.Path = new PropertyPath("Mail");
            txtPhoneBinding.Path = new PropertyPath("Phone");
            txtFirst_NameBinding.Path = new PropertyPath("First_Name");
            txtLast_NameBinding.Path = new PropertyPath("Last_Name");

            txtCategoryBinding.Path = new PropertyPath("Category");
            txtPriceBinding.Path = new PropertyPath("Price");
            txtProductNameBinding.Path = new PropertyPath("ProductName");
            txtQuantityBinding.Path = new PropertyPath("Quantity");

            adressTextBox.SetBinding(TextBox.TextProperty, txtAdressBinding);
            mailTextBox.SetBinding(TextBox.TextProperty, txtMailBinding);
            phoneTextBox.SetBinding(TextBox.TextProperty, txtPhoneBinding);
            first_NameTextBox.SetBinding(TextBox.TextProperty, txtFirst_NameBinding);
            last_NameTextBox.SetBinding(TextBox.TextProperty, txtLast_NameBinding);

            categoryTextBox.SetBinding(TextBox.TextProperty, txtCategoryBinding);
            priceTextBox.SetBinding(TextBox.TextProperty, txtPriceBinding);
            productNameTextBox.SetBinding(TextBox.TextProperty, txtProductNameBinding);
            quantityTextBox.SetBinding(TextBox.TextProperty, txtQuantityBinding);


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            customerOrdersViewSource.Source = ctx.Orders.Local;

            ctx.Customers.Load();
            ctx.Products.Load();
            ctx.Orders.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            cmbCustomers.SelectedValuePath = "CustomerId";
            cmbCustomers.DisplayMemberPath = "First_Name";

            cmbProducts.ItemsSource = ctx.Products.Local;
            cmbProducts.SelectedValuePath = "ProductId";
            cmbProducts.DisplayMemberPath = "ProductName";

            System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.Source = ctx.Products.Local;
            ctx.Products.Load();

            BindDataGrid();
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustomerId equals
                             cust.CustomerId
                             join prod in ctx.Products on ord.ProductId equals
                             prod.ProductId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CustomerId,
                                 ord.ProductId,

                                 cust.First_Name,
                                 cust.Last_Name,
                                 cust.Mail,
                                 cust.Adress,
                                 cust.Phone,

                                 prod.ProductName,
                                 prod.Price,
                                 prod.Category,
                                 prod.Quantity
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        //buttons for customers
        private void custSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {

                    customer = new Customer()
                    {
                        First_Name = first_NameTextBox.Text.Trim(),
                        Last_Name = last_NameTextBox.Text.Trim(),
                        Adress = adressTextBox.Text.Trim(),
                        Mail = mailTextBox.Text.Trim(),
                        Phone = phoneTextBox.Text.Trim()
                    };
                   
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
               
                    ctx.SaveChanges();
                }
               
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                custNew.IsEnabled = true;
                custEdit.IsEnabled = true;
                custSave.IsEnabled = false;
                custCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                custPrev.IsEnabled = true;
                custNext.IsEnabled = true;

                first_NameTextBox.IsEnabled = false;
                last_NameTextBox.IsEnabled = false;
                adressTextBox.IsEnabled = false;
                mailTextBox.IsEnabled = false;
                phoneTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.First_Name = first_NameTextBox.Text.Trim();
                    customer.Last_Name = last_NameTextBox.Text.Trim();
                    customer.Adress = adressTextBox.Text.Trim();
                    customer.Mail = mailTextBox.Text.Trim();
                    customer.Phone = phoneTextBox.Text.Trim();
                
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                
                customerViewSource.View.MoveCurrentTo(customer);

                custNew.IsEnabled = true;
                custEdit.IsEnabled = true;
                custDelete.IsEnabled = true;
                custSave.IsEnabled = false;
                custCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                custPrev.IsEnabled = true;
                custNext.IsEnabled = true;

                first_NameTextBox.IsEnabled = false;
                last_NameTextBox.IsEnabled = false;
                adressTextBox.IsEnabled = false;
                mailTextBox.IsEnabled = false;
                phoneTextBox.IsEnabled = false;

            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                custNew.IsEnabled = true;
                custEdit.IsEnabled = true;
                custDelete.IsEnabled = true;
                custSave.IsEnabled = false;
                custCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                custPrev.IsEnabled = true;
                custNext.IsEnabled = true;

                first_NameTextBox.IsEnabled = false;
                last_NameTextBox.IsEnabled = false;
                adressTextBox.IsEnabled = false;
                mailTextBox.IsEnabled = false;
                phoneTextBox.IsEnabled = false;

                first_NameTextBox.SetBinding(TextBox.TextProperty, txtFirst_NameBinding);
                last_NameTextBox.SetBinding(TextBox.TextProperty, txtLast_NameBinding);
                adressTextBox.SetBinding(TextBox.TextProperty, txtAdressBinding);
                mailTextBox.SetBinding(TextBox.TextProperty, txtMailBinding);
                phoneTextBox.SetBinding(TextBox.TextProperty, txtPhoneBinding);
            }
            SetValidationBinding();
        }

        private void custNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            custNew.IsEnabled = false;
            custEdit.IsEnabled = false;
            custDelete.IsEnabled = false;

            custSave.IsEnabled = true;
            custCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            custPrev.IsEnabled = false;
            custNext.IsEnabled = false;

            first_NameTextBox.IsEnabled = true;
            last_NameTextBox.IsEnabled = true;
            adressTextBox.IsEnabled = true;
            mailTextBox.IsEnabled = true;
            phoneTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(first_NameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(last_NameTextBox, TextBox.TextProperty);
            first_NameTextBox.Text = "";
            last_NameTextBox.Text = "";
            Keyboard.Focus(first_NameTextBox);
        }

        private void custEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempFirstName = first_NameTextBox.Text.ToString();
            string tempLastName = last_NameTextBox.Text.ToString();
            string tempAdress = adressTextBox.Text.ToString();
            string tempMail = mailTextBox.Text.ToString();
            string tempPhone = phoneTextBox.Text.ToString();

            custNew.IsEnabled = false;
            custEdit.IsEnabled = false;
            custDelete.IsEnabled = false;
            custSave.IsEnabled = true;
            custCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            custPrev.IsEnabled = false;
            custNext.IsEnabled = false;

            first_NameTextBox.IsEnabled = true;
            last_NameTextBox.IsEnabled = true;
            adressTextBox.IsEnabled = true;
            mailTextBox.IsEnabled = true;
            phoneTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(first_NameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(last_NameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(adressTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(mailTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneTextBox, TextBox.TextProperty);

            first_NameTextBox.Text = tempFirstName;
            last_NameTextBox.Text = tempLastName;
            adressTextBox.Text = tempAdress;
            mailTextBox.Text = tempMail;
            phoneTextBox.Text = tempPhone;

            Keyboard.Focus(first_NameTextBox);
        }

        private void custCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            custNew.IsEnabled = true;
            custEdit.IsEnabled = true;
            custSave.IsEnabled = false;
            custCancel.IsEnabled = false;
            customerDataGrid.IsEnabled = true;
            custPrev.IsEnabled = true;
            custNext.IsEnabled = true;

            first_NameTextBox.IsEnabled = false;
            last_NameTextBox.IsEnabled = false;
            adressTextBox.IsEnabled = false;
            mailTextBox.IsEnabled = false;
            phoneTextBox.IsEnabled = false;

            first_NameTextBox.SetBinding(TextBox.TextProperty, txtFirst_NameBinding);
            last_NameTextBox.SetBinding(TextBox.TextProperty, txtLast_NameBinding);
            adressTextBox.SetBinding(TextBox.TextProperty, txtAdressBinding);
            mailTextBox.SetBinding(TextBox.TextProperty, txtMailBinding);
            phoneTextBox.SetBinding(TextBox.TextProperty, txtPhoneBinding);

        }

        private void custDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempFirstName = first_NameTextBox.Text.ToString();
            string tempLastName = last_NameTextBox.Text.ToString();
            string tempAdress = adressTextBox.Text.ToString();
            string tempMail = mailTextBox.Text.ToString();
            string tempPhone = phoneTextBox.Text.ToString();

            custNew.IsEnabled = false;
            custEdit.IsEnabled = false;
            custDelete.IsEnabled = false;
            custSave.IsEnabled = true;
            custCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            custPrev.IsEnabled = false;
            custNext.IsEnabled = false;

            BindingOperations.ClearBinding(first_NameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(last_NameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(adressTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(mailTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneTextBox, TextBox.TextProperty);

            first_NameTextBox.Text = tempFirstName;
            last_NameTextBox.Text = tempLastName;
            adressTextBox.Text = tempAdress;
            mailTextBox.Text = tempMail;
            phoneTextBox.Text = tempPhone;
        }

        private void custPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void custNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        //buttons for products

        private void prodSave_Click(object sender, RoutedEventArgs e)
        {
            Product product = null;
            if (action == ActionState.New)
            {
                try
                {
                   
                    product = new Product()
                    {
                        ProductName = productNameTextBox.Text.Trim(),
                        Price = priceTextBox.Text.Trim(),
                        Category = categoryTextBox.Text.Trim(),
                        Quantity = quantityTextBox.Text.Trim()
                    };

                   
                    ctx.Products.Add(product);
                    productViewSource.View.Refresh();
                  
                    ctx.SaveChanges();
                }
                
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                prodNew.IsEnabled = true;
                prodEdit.IsEnabled = true;
                prodSave.IsEnabled = false;
                prodCancel.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                prodPrev.IsEnabled = true;
                prodNext.IsEnabled = true;
                categoryTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;
                productNameTextBox.IsEnabled = false;
                quantityTextBox.IsEnabled = false;
            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    product.Category = categoryTextBox.Text.Trim();
                    product.Price = priceTextBox.Text.Trim();
                    product.ProductName = productNameTextBox.Text.Trim();
                    product.Quantity = quantityTextBox.Text.Trim();

                  
                    ctx.SaveChanges();

                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();
             
                productViewSource.View.MoveCurrentTo(product);
                prodNew.IsEnabled = true;
                prodEdit.IsEnabled = true;
                prodDelete.IsEnabled = true;

                prodSave.IsEnabled = false;
                prodCancel.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                prodPrev.IsEnabled = true;
                prodNext.IsEnabled = true;

                categoryTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;
                productNameTextBox.IsEnabled = false;
                quantityTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
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

                prodNew.IsEnabled = true;
                prodEdit.IsEnabled = true;
                prodDelete.IsEnabled = true;

                prodSave.IsEnabled = false;
                prodCancel.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                prodPrev.IsEnabled = true;
                prodNext.IsEnabled = true;

                categoryTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;
                productNameTextBox.IsEnabled = false;
                quantityTextBox.IsEnabled = false;

                categoryTextBox.SetBinding(TextBox.TextProperty, txtCategoryBinding);
                priceTextBox.SetBinding(TextBox.TextProperty, txtPriceBinding);
                productNameTextBox.SetBinding(TextBox.TextProperty, txtProductNameBinding);
                quantityTextBox.SetBinding(TextBox.TextProperty, txtQuantityBinding);

            }

            SetValidationBinding();
        }

        private void prodNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            prodNew.IsEnabled = false;
            prodEdit.IsEnabled = false;
            prodDelete.IsEnabled = false;

            prodSave.IsEnabled = true;
            prodCancel.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            prodPrev.IsEnabled = false;
            prodNext.IsEnabled = false;

            categoryTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;
            productNameTextBox.IsEnabled = true;
            quantityTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(categoryTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(productNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(quantityTextBox, TextBox.TextProperty);
            categoryTextBox.Text = "";
            priceTextBox.Text = "";
            productNameTextBox.Text = "";
            quantityTextBox.Text = "";
            Keyboard.Focus(productNameTextBox);
        }

        private void prodEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempCategory = categoryTextBox.Text.ToString();
            string tempPrice = priceTextBox.Text.ToString();
            string tempProductName = productNameTextBox.Text.ToString();
            string tempQuantity = quantityTextBox.Text.ToString();

            prodNew.IsEnabled = false;
            prodEdit.IsEnabled = false;
            prodDelete.IsEnabled = false;

            prodSave.IsEnabled = true;
            prodCancel.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            prodPrev.IsEnabled = false;
            prodNext.IsEnabled = false;

            categoryTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;
            productNameTextBox.IsEnabled = true;
            quantityTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(categoryTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(productNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(quantityTextBox, TextBox.TextProperty);
            categoryTextBox.Text = tempCategory;
            priceTextBox.Text = tempPrice;
            productNameTextBox.Text = tempProductName;
            quantityTextBox.Text = tempQuantity;
            Keyboard.Focus(productNameTextBox);
        }

        private void prodCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            prodNew.IsEnabled = true;
            prodEdit.IsEnabled = true;


            prodSave.IsEnabled = false;
            prodCancel.IsEnabled = false;
            productDataGrid.IsEnabled = true;
            prodPrev.IsEnabled = true;
            prodNext.IsEnabled = true;

            categoryTextBox.IsEnabled = false;
            priceTextBox.IsEnabled = false;
            productNameTextBox.IsEnabled = false;
            quantityTextBox.IsEnabled = false;

            categoryTextBox.SetBinding(TextBox.TextProperty, txtCategoryBinding);
            priceTextBox.SetBinding(TextBox.TextProperty, txtPriceBinding);
            productNameTextBox.SetBinding(TextBox.TextProperty, txtProductNameBinding);
            quantityTextBox.SetBinding(TextBox.TextProperty, txtQuantityBinding);
        }

        private void prodDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempCategory = categoryTextBox.Text.ToString();
            string tempPrice = priceTextBox.Text.ToString();
            string tempProductName = productNameTextBox.Text.ToString();
            string tempQuantity = quantityTextBox.Text.ToString();

            prodNew.IsEnabled = false;
            prodEdit.IsEnabled = false;
            prodDelete.IsEnabled = false;

            prodSave.IsEnabled = true;
            prodCancel.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            prodPrev.IsEnabled = false;
            prodNext.IsEnabled = false;

            BindingOperations.ClearBinding(categoryTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(productNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(quantityTextBox, TextBox.TextProperty);
            categoryTextBox.Text = tempCategory;
            priceTextBox.Text = tempPrice;
            productNameTextBox.Text = tempProductName;
            quantityTextBox.Text = tempQuantity;
        }

        private void prodPrev_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToPrevious();
        }


        private void prodNext_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToNext();
        }

        //butoane pentru orders

        private void ordNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            ordNew.IsEnabled = false;
            ordEdit.IsEnabled = false;
            ordDelete.IsEnabled = false;

            ordSave.IsEnabled = true;
            ordCancel.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            ordPrev.IsEnabled = false;
            ordNext.IsEnabled = false;

            cmbCustomers.IsEnabled = true;
            cmbProducts.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProducts, ComboBox.TextProperty);
            cmbCustomers.Text = "";
            cmbProducts.Text = "";
            Keyboard.Focus(cmbCustomers);
        }

        private void ordEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempCustomer = cmbCustomers.Text.ToString();
            string tempProduct = cmbProducts.Text.ToString();

            ordNew.IsEnabled = false;
            ordEdit.IsEnabled = false;
            ordDelete.IsEnabled = false;

            ordSave.IsEnabled = true;
            ordCancel.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            ordPrev.IsEnabled = false;
            ordNext.IsEnabled = false;

            cmbCustomers.IsEnabled = true;
            cmbProducts.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProducts, ComboBox.TextProperty);
            cmbCustomers.Text = tempCustomer;
            cmbProducts.Text = tempProduct;
            Keyboard.Focus(cmbCustomers);
        }

        private void ordDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempCustomer = cmbCustomers.Text.ToString();
            string tempProducts = cmbProducts.Text.ToString();

            ordNew.IsEnabled = false;
            ordEdit.IsEnabled = false;
            ordDelete.IsEnabled = false;

            ordSave.IsEnabled = true;
            ordCancel.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            ordPrev.IsEnabled = false;
            ordNext.IsEnabled = false;


            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProducts, ComboBox.TextProperty);
            cmbCustomers.Text = tempCustomer;
            cmbProducts.Text = tempProducts;
        }

        private void ordSave_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Product product = (Product)cmbProducts.SelectedItem;

                    //instantiem Order entity
                    order = new Order()
                    {
                        CustomerId = customer.CustomerId,
                        ProductId = product.ProductId

                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                ordNew.IsEnabled = true;
                ordEdit.IsEnabled = true;
                ordSave.IsEnabled = false;
                ordCancel.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                ordPrev.IsEnabled = true;
                ordNext.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbProducts.IsEnabled = false;

            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustomerId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.ProductId = Int32.Parse(cmbProducts.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                customerViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(order);
                ordNew.IsEnabled = true;
                ordEdit.IsEnabled = true;
                ordDelete.IsEnabled = true;

                ordSave.IsEnabled = false;
                ordCancel.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                ordPrev.IsEnabled = true;
                ordNext.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbProducts.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();
                ordNew.IsEnabled = true;
                ordEdit.IsEnabled = true;
                ordDelete.IsEnabled = true;

                ordSave.IsEnabled = false;
                ordCancel.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                ordPrev.IsEnabled = true;
                ordNext.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbProducts.IsEnabled = false;

                cmbCustomers.SetBinding(ComboBox.TextProperty, txtCustomer);
                cmbProducts.SetBinding(ComboBox.TextProperty, txtProduct);
            }

        }

        private void ordCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            ordNew.IsEnabled = true;
            ordEdit.IsEnabled = true;
            ordSave.IsEnabled = false;
            ordCancel.IsEnabled = false;
            ordersDataGrid.IsEnabled = true;
            ordPrev.IsEnabled = true;
            ordNext.IsEnabled = true;

            cmbCustomers.IsEnabled = false;
            cmbProducts.IsEnabled = false;

            cmbCustomers.SetBinding(ComboBox.TextProperty, txtCustomer);
            cmbProducts.SetBinding(ComboBox.TextProperty, txtProduct);
        }

        private void ordPrev_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void ordNext_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }


        private void SetValidationBinding()
        {
            Binding first_NameValidationBinding = new Binding();
            first_NameValidationBinding.Source = customerViewSource;
            first_NameValidationBinding.Path = new PropertyPath("FirstName");
            first_NameValidationBinding.NotifyOnValidationError = true;
            first_NameValidationBinding.Mode = BindingMode.TwoWay;
            first_NameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            first_NameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            first_NameTextBox.SetBinding(TextBox.TextProperty, first_NameValidationBinding);


            Binding last_NameValidationBinding = new Binding();
            last_NameValidationBinding.Source = customerViewSource;
            last_NameValidationBinding.Path = new PropertyPath("LastName");
            last_NameValidationBinding.NotifyOnValidationError = true;
            last_NameValidationBinding.Mode = BindingMode.TwoWay;
            last_NameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            last_NameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            last_NameTextBox.SetBinding(TextBox.TextProperty, last_NameValidationBinding); 

          


        }

    }
}

          