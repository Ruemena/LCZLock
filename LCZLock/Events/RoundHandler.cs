﻿using Exiled.API.Features;
using Exiled.API.Enums;


using Exiled.Events.EventArgs.Player;

namespace Exiled.LCZLockEvents
{
    using Exiled.Events.EventArgs.Server;
    using LCZLock;
    using MEC;

    internal sealed class RoundHandler
    {
        private readonly LCZLock Instance = LCZLock.Instance;
        public void OnRoundStarted()
        // by default, is allowed
        {
            IEnumerable<Door> allDoors = Door.List;
            
            Lift lczA = Lift.Get(ElevatorType.LczA);
            Lift lczB = Lift.Get(ElevatorType.LczB);
            foreach (var door in allDoors)
            {
                if ((door.Type == DoorType.ElevatorLczA || door.Type == DoorType.ElevatorLczB) && !door.IsLocked)
                {
                    door.ChangeLock(DoorLockType.AdminCommand);
                }
            }
            Timing.RunCoroutine(UnlockElevators(Instance.Config.TimeUntilUnlock), "unlockLczElevators");
        }

        public void OnRoundEnded(RoundEndedEventArgs ev) => Timing.KillCoroutines("unlockLczElevators");
        private IEnumerator<float> UnlockElevators(float waitSeconds)
        {
            yield return Timing.WaitForSeconds(waitSeconds);
            IEnumerable<Door> allDoors = Door.List;

            foreach (var door in allDoors)
            {
                if ((door.Type == DoorType.ElevatorLczA || door.Type == DoorType.ElevatorLczB) && door.IsLocked)
                {
                    door.ChangeLock(DoorLockType.None);
                }
            }
            if (Instance.Config.DoCassieMessage)
            {
                Cassie.MessageTranslated(Instance.Config.CassieMessage, Instance.Config.Subtitles, true, false, true); // message, subtitles, hold, not noisy, do subtitles
            };
        }
    }
}