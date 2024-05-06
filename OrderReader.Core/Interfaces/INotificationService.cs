using System.Threading.Tasks;
using OrderReader.Core.Enums;

namespace OrderReader.Core.Interfaces;

public interface INotificationService
{
    Task<DialogResult> ShowMessage(string title, string message, string button = "OK");
    Task<DialogResult> ShowQuestion(string title, string message, string primaryButton = "Yes", string secondaryButton = "No");
    Task<string> ShowConfigMessage(string title, string message);
    Task<string> ShowConfigMessage();
    Task<DialogResult> ShowUpdateNotification(string updatedVersion);
}