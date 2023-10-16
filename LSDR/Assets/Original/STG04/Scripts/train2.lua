require "dreams"
train = require "train"

player = GetEntity("__player")
audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 3
completedMotion = false
interacted = false

function start()
    train.generateTargetsFrom(1)
    this.SetChildVisible(false)
end

function intervalUpdate()
    if not interacted then return end

    local playerDistance = (player.WorldPosition - this.GameObject.WorldPosition).length()
    if playerDistance < 1 then
        interacted = false
        DreamSystem.SetNextTransitionDream(dreams.ClockworkMachines)
        DreamSystem.TransitionToDream()
    end
end

function interact()
    interacted = true
    this.SetChildVisible(true)
    this.LogGraphContribution(-9, 5)
    audio.Play()
    this.PlayAnimation(0)
    this.Action
        .Do(function() completedMotion = train.moveTowardsTarget(this, moveSpeed) end)
        .Until(Condition.Custom(|| completedMotion))
        .Then(|| train.incrementTarget(this))
        .ThenLoop()
end

