require "dreams"

audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio
player = GetEntity("__player")

function interact()
    audio.Play()
    this.PlayAnimation(0)
    
    local target = player.WorldPosition
    target.y = this.GameObject.WorldPosition.y
    this.LookAt(target)

    this.Action
        .WaitUntil(Condition.WaitForSeconds(0.7))
        .Then(|| this.StopAnimation())
        .Then(|| SetCanControlPlayer(false))
        .Then(function()
            DreamSystem.SetNextTransitionDream(dreams.Void)
            DreamSystem.TransitionToDream()
        end)
        .ThenFinish()
end