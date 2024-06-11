using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class UnityServicesController : MonoBehaviour
{
    public static ServicesInitializationState State;
    public static UnityServicesController Instance;

    public string environment = "production";

    private async void Awake()
    {
        try
        {
            InitializationOptions options = new InitializationOptions().SetEnvironmentName(environment);
            await UnityServices.InitializeAsync(options);
            await SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async Task SignInAnonymouslyAsync()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (AuthenticationService.Instance.IsSignedIn)
        {
            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            // if not, then do nothing
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
