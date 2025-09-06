namespace MovieApp.Utilities
{
    public static class ToastNotification
    {
        /// <summary>
        /// Sets a success toast notification in TempData
        /// </summary>
        /// <param name="tempData">Controller's TempData</param>
        /// <param name="message">Message to display</param>
        /// <param name="title">Optional title for the toast</param>
        public static void Success(dynamic tempData, string message, string title = "Success")
        {
            SetToast(tempData, message, "success", title);
        }
        
        /// <summary>
        /// Sets an error toast notification in TempData
        /// </summary>
        /// <param name="tempData">Controller's TempData</param>
        /// <param name="message">Message to display</param>
        /// <param name="title">Optional title for the toast</param>
        public static void Error(dynamic tempData, string message, string title = "Error")
        {
            SetToast(tempData, message, "danger", title);
        }
        
        /// <summary>
        /// Sets a warning toast notification in TempData
        /// </summary>
        /// <param name="tempData">Controller's TempData</param>
        /// <param name="message">Message to display</param>
        /// <param name="title">Optional title for the toast</param>
        public static void Warning(dynamic tempData, string message, string title = "Warning")
        {
            SetToast(tempData, message, "warning", title);
        }
        
        /// <summary>
        /// Sets an info toast notification in TempData
        /// </summary>
        /// <param name="tempData">Controller's TempData</param>
        /// <param name="message">Message to display</param>
        /// <param name="title">Optional title for the toast</param>
        public static void Info(dynamic tempData, string message, string title = "Information")
        {
            SetToast(tempData, message, "info", title);
        }
        
        /// <summary>
        /// Sets toast notification properties in TempData
        /// </summary>
        /// <param name="tempData">Controller's TempData</param>
        /// <param name="message">Message to display</param>
        /// <param name="type">Type of toast</param>
        /// <param name="title">Title for the toast</param>
        private static void SetToast(dynamic tempData, string message, string type, string title)
        {
            tempData["ToastMessage"] = message;
            tempData["ToastType"] = type;
            tempData["ToastTitle"] = title;
        }
    }
}