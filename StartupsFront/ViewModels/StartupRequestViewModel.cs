using Newtonsoft.Json;
using StartupsFront.Models;
using StartupsFront.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StartupsFront.ViewModels
{
    public class StartupRequestViewModel : BaseViewModel
    {
        public StartupModel Startup { get; set; }

        public UserModel User { get; set; }
        public UserModel Me { get; set; }
        public Command AcceptCmd { get; }
        public Command RejectCmd { get; }

        public override string ErrorMessage
        {
            get => base.ErrorMessage;
            set
            {
                ErrorMessageAct?.Invoke(value);
                base.ErrorMessage = value;
            }
        }

        public override string SuccessMessage
        {
            get => base.SuccessMessage;
            set
            {
                SuccessMessageAct?.Invoke(value);
                base.SuccessMessage = value;
            }
        }

        public event Action<string> SuccessMessageAct;
        public event Action<string> ErrorMessageAct;
        public event Action<StartupRequestViewModel> NeedToRemoveMe;

        public StartupRequestViewModel()
        {
            AcceptCmd = new Command(async () => await AcceptTask());
            RejectCmd = new Command(async () => await RejectTask());
            Me = DataStore.MainModel.UserOrNull;
        }
        private async Task AcceptTask()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var hash = await Requests.CalculateStartupsHash(Me.Name, Me.Token);

                    var uri = Requests.AcceptUserToStartup(Me.Id, hash, Startup.Id, User.Id);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var answerDefinition = new { Result = "" };

                        var answer = JsonConvert.DeserializeAnonymousType(responseString, answerDefinition);

                        if (answer.Result.Equals("SuccessJoined", StringComparison.OrdinalIgnoreCase))
                            SuccessMessage = answer.Result;

                        else ErrorMessage = answer.Result;

                        NeedToRemoveMe?.Invoke(this);
                    }
                    catch
                    {
                        ErrorMessage = responseString;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }

        private async Task RejectTask()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var hash = await Requests.CalculateStartupsHash(Me.Name, Me.Token);

                    var uri = Requests.RejectUserToStartup(Me.Id, hash, Startup.Id, User.Id);

                    var response = await client.GetAsync(uri);

                    var responseString = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var answerDefinition = new { Result = "" };

                        var answer = JsonConvert.DeserializeAnonymousType(responseString, answerDefinition);

                        if (answer.Result.Equals("SuccessDenied", StringComparison.OrdinalIgnoreCase))
                            SuccessMessage = answer.Result;

                        else ErrorMessage = answer.Result;

                        NeedToRemoveMe?.Invoke(this);
                    }
                    catch
                    {
                        ErrorMessage = responseString;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }
    }
}
