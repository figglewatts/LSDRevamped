require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 3
front = this.Forward
back = this.Forward.negated()

function start()
    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 0.2 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.Happytown)
        DreamSystem.TransitionToDream()
    end

    this.SnapToFloor()
end

function interact()
    this.SetChildVisible(true)
    interacted = true
    this.LogGraphContribution(-3, 1)
    audio.Play()

    this.Action
        .Do(|| this.LookInDirection(front))
        .Then(|| this.MoveInDirection(front, moveSpeed))
        .Until(Condition.WaitForSeconds(10))
        .Then(|| this.LookInDirection(back))
        .Then(|| this.MoveInDirection(back, moveSpeed))
        .Until(Condition.WaitForSeconds(10))
        .ThenLoop()
end