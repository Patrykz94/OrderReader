using System.Threading.Tasks;

namespace OrderReader.Core.Interfaces;

public interface IUserNotificationService
{
    Task<DialogResult> ShowMessage(string title, string message, string button = "OK");
    Task<DialogResult> ShowQuestion(string title, string message, string primaryButton = "Yes", string secondaryButton = "No");
}