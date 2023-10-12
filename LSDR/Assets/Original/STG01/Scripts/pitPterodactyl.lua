require "dreams"

player = GetEntity("__player")
audio = GetEntity("PterodactylAudio").DreamAudio
screechAudio = GetEntity("PterodactylScreech").DreamAudio

state = "hidden"
moveSpeed = 0.75
linked = false
distanceToPlayer = 0

function start()
    this.SetChildVisible(false)
end

function intervalUpdate()
    distanceToPlayer = (player.WorldPosition - this.GameObject.WorldPosition).length()
end

function update()
    if state == "hidden" or linked then return end

    if distanceToPlayer < 0.5 then
        linked = true
        DreamSystem.SetNextTransitionDream(dreams.PitAndTemple)
        DreamSystem.TransitionToDream()
    end

    if state == "chasing" then
        local playerPositionFlat = player.WorldPosition
        playerPositionFlat.y = this.GameObject.WorldPosition.y
        this.LookTowards(playerPositionFlat, 5)
        this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)

        if distanceToPlayer > 5 then
            state = "flying"
        end

        return
    end

    -- otherwise fly in a circle
    if this.GameObject.WorldPosition.y < 2 then
        this.MoveInDirection(this.GameObject.UpDirection, moveSpeed)
    end
    this.LookTowards(this.GameObject.WorldPosition + this.GameObject.RightDirection, 10)
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)
end

function interact()
    this.SetChildVisible(true)
    this.PlayAnimation(0)
    audio.Play()
    screechAudio.Play()
    DreamSystem.LogGraphContributionFromEntity(-5, 0)

    state = "chasing"
end