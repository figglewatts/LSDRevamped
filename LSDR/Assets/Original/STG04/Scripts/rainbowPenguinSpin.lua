require "dreams"

player = GetEntity("__player")

interacted = false
moveSpeed = 0.3

function start()
    if IsDayEven() then
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
end

function update()
    if not interacted then return end

    this.LookTowards(this.GameObject.WorldPosition + this.GameObject.RightDirection, 10)
end

function interact()
    this.SetChildVisible(true)
    interacted = true
    DreamSystem.LogGraphContributionFromEntity(1, 1)
    this.PlayAnimation(0)
end