using System;
using Microsoft.Win32;

namespace DCSBackupTool
{
    class RegistryManipulator
    {
        public static bool WriteRegistry(RegistryKey baseRegistryKey, string subKey,
                string KeyName, object Value)
        {
            try
            {
                RegistryKey rk = baseRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(subKey);
                sk1.SetValue(KeyName.ToUpper(), Value);   // Save the value
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string ReadRegistry(RegistryKey baseRegistryKey, string subKey, string KeyName)
        {  
            // Opening the registry key
            RegistryKey rk = baseRegistryKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(subKey);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}
