require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
playerMoveAcceleration = 0.4
playerMoveSpeed = 7.5
currentPlayerMoveSpeed = 0

function interact()
    this.LogGraphContribution(4, 1)
    interacted = true
    audio.Play()

    if IsDayEven() then
        -- move forward and link to natural world
        this.Action
            .Do(|| SetCanControlPlayer(false))
            .Then(|| player.LookAtPlane(player.WorldPosition + Unity.Vector3(1, 0, 0)))
            .Then(|| this.PlayAnimation(0))
            .ThenWaitUntil(this.WaitForAnimation(0))
            .Then(|| this.StopAnimation())
            .Then(|| this.SetChildVisible(false))
            .Then(|| movePlayerForward())
            .Until(Condition.WaitForSeconds(2))
            .Then(|| link())
            .ThenFinish()
    else
        -- push player back
        this.Action
            .Do(|| SetCanControlPlayer(false))
            .Then(|| player.LookAtPlane(player.WorldPosition + Unity.Vector3(1, 0, 0)))
            .Then(|| this.PlayAnimation(0))
            .ThenWaitUntil(this.WaitForAnimation(0))
            .Then(|| this.StopAnimation())
            .Then(|| this.SetChildVisible(false))
            .Then(|| movePlayerBack())
            .Until(Condition.WaitForSeconds(5))
            .Then(|| SetCanControlPlayer(true))
            .ThenFinish()
    end
end

function movePlayerBack()
    currentPlayerMoveSpeed = currentPlayerMoveSpeed + playerMoveAcceleration
    if currentPlayerMoveSpeed > playerMoveSpeed then
        currentPlayerMoveSpeed = playerMoveSpeed
    end
    player.WorldPosition = player.WorldPosition - (Unity.Vector3(currentPlayerMoveSpeed, 0, 0) * Unity.DeltaTime())
end

function movePlayerForward()
    currentPlayerMoveSpeed = currentPlayerMoveSpeed + (playerMoveAcceleration / 10)
    if currentPlayerMoveSpeed > playerMoveSpeed / 10 then
        currentPlayerMoveSpeed = playerMoveSpeed / 10
    end
    player.WorldPosition = player.WorldPosition + (Unity.Vector3(currentPlayerMoveSpeed, 0, 0) * Unity.DeltaTime())
end

function link()
    DreamSystem.SetNextTransitionDream(dreams.NaturalWorld)
    DreamSystem.TransitionToDream()
end