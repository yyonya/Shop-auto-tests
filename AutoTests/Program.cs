using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Npgsql;

class Program
{
    static string connString = "Host=localhost;Port=50000;Username=server;Password=qtzxc1;Database=mts";

    static void Main(string[] args)
    {

        while (true)
        {
            int testIdCounter = GetMaxTestId();

            var options = new FirefoxOptions();
            options.AddArgument("--headless");
            IWebDriver driver = new FirefoxDriver(options);
            
            string currentStep = "initializing";

            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                DateTime startTimestampOpening = DateTime.UtcNow;
                currentStep = "opening";
                driver.Navigate().GoToUrl("https://shop.mts.by/");
                LogOperation(startTimestampOpening, DateTime.UtcNow, "opening", "passed", string.Empty, testIdCounter);

                // cookies
                DateTime startTimestampCookie = DateTime.UtcNow;
                currentStep = "cookie";
                try
                {
                    IWebElement cookieBanner = wait.Until(e => e.FindElement(By.ClassName("cookie")));
                    IWebElement acceptCookieButton = cookieBanner.FindElement(By.XPath("//button[contains(text(),'Принять')]"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", acceptCookieButton);
                    LogOperation(startTimestampCookie, DateTime.UtcNow, "cookie", "passed", string.Empty, testIdCounter);
                }
                catch (WebDriverTimeoutException)
                {
                    LogOperation(startTimestampCookie, DateTime.UtcNow, "cookie", "failed", "Баннер cookies не найден", testIdCounter);
                }

                // "Каталог"
                DateTime startTimestampCatalog = DateTime.UtcNow;
                currentStep = "catalog";
                try
                {
                    IWebElement catalogButton = wait.Until(e => e.FindElement(By.XPath("//button[contains(text(),'Каталог')]")));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", catalogButton);
                    LogOperation(startTimestampCatalog, DateTime.UtcNow, "catalog", "passed", string.Empty, testIdCounter);
                }
                catch (WebDriverTimeoutException)
                {
                    LogOperation(startTimestampCatalog, DateTime.UtcNow, "catalog", "failed", "Кнопка 'Каталог' не найдена", testIdCounter);
                }

                // "Все"
                DateTime startTimestampAll = DateTime.UtcNow;
                currentStep = "all";
                try
                {
                    IWebElement allLink = wait.Until(e => e.FindElement(By.XPath("//a[@class='menu-category__title' and contains(text(),'Все')]")));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", allLink);
                    LogOperation(startTimestampAll, DateTime.UtcNow, "all", "passed", string.Empty, testIdCounter);
                }
                catch (WebDriverTimeoutException)
                {
                    LogOperation(startTimestampAll, DateTime.UtcNow, "all", "failed", "Ссылка 'Все' не найдена", testIdCounter);
                }

                // "Apple"
                DateTime startTimestampApple = DateTime.UtcNow;
                currentStep = "apple";
                try
                {
                    IWebElement appleLink = wait.Until(e => e.FindElement(By.XPath("//a[contains(@class,'catalog-item-n') and contains(.,'Apple')]")));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", appleLink);
                    LogOperation(startTimestampApple, DateTime.UtcNow, "apple", "passed", string.Empty, testIdCounter);
                }
                catch (WebDriverTimeoutException)
                {
                    LogOperation(startTimestampApple, DateTime.UtcNow, "apple", "failed", "Ссылка 'Apple' не найдена", testIdCounter);
                }

                // "iPhone"
                Random random = new Random();
                int randomNumber = random.Next(11, 16);
                // Console.WriteLine($"Число '{randomNumber}");
                string xpath = $"//a[contains(@class,'catalog-item-n') and contains(@href,'/phones/apple/iphone-{randomNumber}/')]";
                Thread.Sleep(3000);
                DateTime startTimestampIphone = DateTime.UtcNow;
                currentStep = "iphone";
                try
                {
                    IWebElement iphone = wait.Until(e => e.FindElement(By.XPath(xpath)));
                    iphone.Click();
                    LogOperation(startTimestampIphone, DateTime.UtcNow, "iphone", "passed", string.Empty, testIdCounter);
                }
                catch (WebDriverTimeoutException)
                {
                    LogOperation(startTimestampIphone, DateTime.UtcNow, "iphone", "failed", "Ссылка на 'iPhone' не найдена", testIdCounter);
                }

                // элемент с текстом iPhone
                DateTime startTimestampElement = DateTime.UtcNow;
                currentStep = "element";
                try
                {
                    IWebElement iphoneLink = wait.Until(e => e.FindElement(By.XPath("//a[contains(@class,'linkTovar') and contains(text(),'iPhone')]")));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", iphoneLink);
                    LogOperation(startTimestampElement, DateTime.UtcNow, "element", "passed", string.Empty, testIdCounter);
                    string iphoneName = iphoneLink.Text;

                    // "Добавить в корзину"
                    DateTime startTimestampAddToCart = DateTime.UtcNow;
                    currentStep = "add-to-cart";
                    try
                    {
                        IWebElement addToCartButton = wait.Until(e => e.FindElement(By.XPath("/html/body/main/div[3]/div[2]/div/div[1]/div/div[1]/div[2]/div/div[2]/div[2]/div[1]/div/div[4]/div[3]/div/a[2]")));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addToCartButton);
                        LogOperation(startTimestampAddToCart, DateTime.UtcNow, "add-to-cart", "passed", string.Empty, testIdCounter);
                        Console.WriteLine($"Товар '{iphoneName}' добавлен в корзину");

                        // "Перейти в корзину"
                        DateTime startTimestampGoToCartAndCheck = DateTime.UtcNow;
                        currentStep = "go-to-cart-and-check";
                        try
                        {
                            IWebElement goToCartButton = wait.Until(e => e.FindElement(By.XPath("//a[@id='addToBasket']//span[text()='Перейти в корзину']")));
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", goToCartButton);

                            // Проверить, что на странице присутствует текст названия элемента каталога
                            bool isItemInCart = driver.PageSource.Contains(iphoneName);
                            if (isItemInCart)
                            {
                                LogOperation(startTimestampGoToCartAndCheck, DateTime.UtcNow, "go-to-cart-and-check", "passed", string.Empty, testIdCounter);

                                Console.WriteLine($"Товар '{iphoneName}' успешно добавлен в корзину.");
                                LogTestsPassed();
                            }
                            else
                            {
                                LogTestsError($"Товар '{iphoneName}' не найден в корзине.");
                            }
                        }
                        catch (WebDriverTimeoutException)
                        {
                            LogOperation(startTimestampGoToCartAndCheck, DateTime.UtcNow, "go-to-cart-and-check", "failed", "Кнопка 'Перейти в корзину' не найдена", testIdCounter);
                            LogTestsError($"Кнопка 'Перейти в корзину' для товара '{iphoneName} не найдена");
                        }
                    }
                    catch (WebDriverTimeoutException)
                    {
                        LogOperation(startTimestampAddToCart, DateTime.UtcNow, "add-to-cart", "failed", $"Кнопка 'Добавить в корзину' для товара '{iphoneName}' не найдена", testIdCounter);
                        LogTestsError($"Кнопка 'Добавить в корзину' для товара '{iphoneName}' не найдена");
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    LogOperation(startTimestampElement, DateTime.UtcNow, "element", "failed", "Ссылка на элемент 'iPhone' не найдена", testIdCounter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка на шаге '{currentStep}': {ex.Message}");
                LogTestsError($"'{currentStep}': {ex.Message}");
            }
            finally
            {
                driver.Quit();
            }

            Thread.Sleep(TimeSpan.FromMinutes(5));
            // Thread.Sleep(TimeSpan.FromSeconds(1));

        }
    }


    static void LogOperation(DateTime startTimestamp, DateTime endTimestamp, string operationCode, string operationResult, string errorDescription, int testId)
    {
        int durationMs = (int)(endTimestamp - startTimestamp).TotalMilliseconds;

        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO OPERATIONS (start_timestamp, end_timestamp, duration_ms, operation_code, operation_result, error_description, test_id) " +
                                  "VALUES (@start, @end, @duration, @code, @result, @error, @testId)";
                cmd.Parameters.AddWithValue("start", startTimestamp);
                cmd.Parameters.AddWithValue("end", endTimestamp);
                cmd.Parameters.AddWithValue("duration", durationMs);
                cmd.Parameters.AddWithValue("code", operationCode);
                cmd.Parameters.AddWithValue("result", operationResult);
                cmd.Parameters.AddWithValue("error", errorDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("testId", testId);
                cmd.ExecuteNonQuery();
            }
        }
    }


    static void LogTestsPassed()
    {
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO TESTS (test_result, error_description) VALUES (@result, @error)";
                cmd.Parameters.AddWithValue("result", "passed");
                cmd.Parameters.AddWithValue("error", DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }


    static void LogTestsError(string err)
    {
        using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO TESTS (test_result, error_description) VALUES (@result, @error)";
                        cmd.Parameters.AddWithValue("result", "failed");
                        cmd.Parameters.AddWithValue("error", err);
                        cmd.ExecuteNonQuery();
                    }
                }
    }


    static int GetMaxTestId()
    {
        using (var conn = new NpgsqlConnection(connString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT COALESCE(MAX(test_id), 0) FROM TESTS";
                var result = cmd.ExecuteScalar(); 
                return (result != null) ? (int)result + 1 : 1; 
            }
        }
    }
}
