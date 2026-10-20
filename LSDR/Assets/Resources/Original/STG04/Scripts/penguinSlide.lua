require "dreams"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

interacted = false
moveSpeed = 5
front = this.Forward
back = this.Forward.negated()

function start()
    local firstGroup = string.sub(this.GameObject.Name, 1, 8) == "Penguin1"
    if IsDayEven() ~= firstGroup then
        this.GameObject.SetActive(false)
        return
    end

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
    if interacted then return end
    this.SetChildVisible(true)
    interacted = true
    this.LogGraphContribution(-3, 1)
    audio.Play()

    this.Action
        .Do(|| this.LookInDirection(front))
        .Then(|| this.MoveInDirection(front, moveSpeed))
        .Until(Condition.WaitForSeconds(4.25))
        .Then(|| this.LookInDirection(back))
        .Then(|| this.MoveInDirection(back, moveSpeed))
        .Until(Condition.WaitForSeconds(5))
        .Then(function()
            this.Rotate(Unity.Vector3(0, 360, 0))
            this.MoveInDirection(this.Forward, moveSpeed)
        end)
        .Until(Condition.WaitForSeconds(0.75))
        .Then(|| this.GameObject.SetActive(false))
        .ThenFinish()
end