﻿using System;
using Ninject;

namespace OrderReader.Core
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        #endregion

        #region Construction

        /// <summary>
        /// Set up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all
        ///       services can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all required view models
            BindViewModels();

            // Create the OrdersLibrary
            SetupOrdersLibrary();

            // Create the CustomersHandler
            SetupCustomersHandler();
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            // Bind to a single instance of Application view model
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }

        /// <summary>
        /// Creates a new instance of <see cref="OrdersLibrary"/>
        /// </summary>
        private static void SetupOrdersLibrary()
        {
            // Set up a single instance of the OrdersHandler
            Kernel.Bind<OrdersLibrary>().ToConstant(new OrdersLibrary());
        }

        /// <summary>
        /// Creates a new instance of <see cref="CustomersHandler"/>
        /// </summary>
        private static void SetupCustomersHandler()
        {
            // Set up a single instance of the OrdersHandler
            Kernel.Bind<CustomersHandler>().ToConstant(new CustomersHandler());
        }


        #endregion

        /// <summary>
        /// Get's a service from the IoC, of he specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        /// <summary>
        /// Quicker way to get the customers handler instance
        /// </summary>
        /// <returns><see cref="CustomersHandler"/> object instance</returns>
        public static CustomersHandler Customers()
        {
            return Kernel.Get<CustomersHandler>();
        }
    }
}
