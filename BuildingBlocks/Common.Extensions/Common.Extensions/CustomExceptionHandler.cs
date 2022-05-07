using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions
{
    public static class CustomExceptionHandler
    {
        public static Models.ErrorLog Parse(this Exception ex)
        {
            var error = new Models.ErrorLog();

            // Get Message & StackTrace
            var message = new StringBuilder();
            var stackTrace = new StringBuilder();
            message.Append(ex.Message);
            stackTrace.Append(ex.StackTrace);
            if (ex.InnerException != null)
            {
                if (ex.Message != ex.InnerException.Message)
                {
                    message.Append(Environment.NewLine);
                    message.Append("------Inner Exception------");
                    message.Append(Environment.NewLine);
                    message.Append(ex.InnerException.Message);

                    stackTrace.Append(Environment.NewLine);
                    stackTrace.Append("------Inner Exception------");
                    stackTrace.Append(Environment.NewLine);
                    stackTrace.Append(ex.InnerException.StackTrace);
                }
                if (ex.InnerException.InnerException != null)
                {
                    message.Append(Environment.NewLine);
                    message.Append("------Inner Exception------");
                    message.Append(Environment.NewLine);
                    message.Append(ex.InnerException.InnerException.Message);

                    stackTrace.Append(Environment.NewLine);
                    stackTrace.Append("------Inner Exception------");
                    stackTrace.Append(Environment.NewLine);
                    stackTrace.Append(ex.InnerException.InnerException.StackTrace);

                    if (ex.InnerException.InnerException.InnerException != null)
                    {
                        message.Append(Environment.NewLine);
                        message.Append("------Inner Exception------");
                        message.Append(Environment.NewLine);
                        message.Append(ex.InnerException.InnerException.InnerException.Message);

                        stackTrace.Append(Environment.NewLine);
                        stackTrace.Append("------Inner Exception------");
                        stackTrace.Append(Environment.NewLine);
                        stackTrace.Append(ex.InnerException.InnerException.InnerException.StackTrace);
                    }
                }
            }
            error.Message = message.ToString();
            error.StackTrace = stackTrace.ToString();

            error.CompleteException = ex.ToString();

            // Get Additonal Details based on the exception type
            if (ex.GetType() == typeof(WebException))
            {
                WebException wex = (WebException)ex;
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            error.AdditionalDetails = reader.ReadToEnd();
                        }
                    }
                }
            }
            else if (ex.GetType() == typeof(DbUpdateException))
            {
                var validationEx = new StringBuilder();

                validationEx.Append("------DbUpdateException------");
                validationEx.Append(Environment.NewLine);

                var dbUpdateEx = ex as DbUpdateException;
                validationEx.Append(dbUpdateEx.Message);
                if (dbUpdateEx.InnerException != null)
                {
                    if (dbUpdateEx.Message != dbUpdateEx.InnerException.Message)
                    {
                        validationEx.Append(Environment.NewLine);
                        validationEx.Append("------Inner Exception------");
                        validationEx.Append(dbUpdateEx.InnerException.Message);
                    }
                    if (dbUpdateEx.InnerException.InnerException != null)
                    {
                        validationEx.Append(Environment.NewLine);
                        validationEx.Append("------Inner Exception------");
                        validationEx.Append(dbUpdateEx.InnerException.InnerException.Message);

                        if (dbUpdateEx.InnerException.InnerException != null)
                        {
                            validationEx.Append(Environment.NewLine);
                            validationEx.Append("------Inner Exception------");
                            validationEx.Append(dbUpdateEx.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
                error.AdditionalDetails = validationEx.ToString();
            }

            return error;
        }
    }
}
