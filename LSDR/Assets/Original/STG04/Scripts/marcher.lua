require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 0.2

function start()
    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.3 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.MonumentPark)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor()
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    DreamSystem.LogGraphContributionFromEntity(3, 3)
    
    audio.Play()

    this.PlayAnimation(0)
    this.Action
        .Do(|| this.MoveInDirection(this.Forward, moveSpeed))
        .Until(Condition.WaitForSeconds(50))
        .Then(|| this.LookInDirection(this.Right))
        .ThenLoop()
end