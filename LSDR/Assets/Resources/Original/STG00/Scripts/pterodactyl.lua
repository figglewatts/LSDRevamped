player = GetEntity("__player")
audioPlayer = GetEntity("PterodactylAudio").DreamAudio

moveSpeed = 0.9
turnSpeed = 180

function start()
    if IsDayEven() or not IsDayLinear(3, 0) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function interact()
    SetCanControlPlayer(false)
    this.LogGraphContribution(-2, 5)
    audioPlayer.Play()
    this.LookAtPlane(player.WorldPosition)

    this.Action
        .Do(flyAtPlayer)
        .Until(Condition.WaitForSeconds(1.5))
        .Then(|| DreamSystem.TransitionToDream())
        .ThenFinish()
end

function flyAtPlayer()
    this.LookTowards(player.WorldPosition, turnSpeed)
    this.MoveInDirection(this.Forward, moveSpeed)
end