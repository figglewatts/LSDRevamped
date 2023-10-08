moveSpeed = 0.2

function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function update()
    local delta = Unity.DeltaTime()
    local position = this.GameObject.WorldPosition
    position.x = position.x - moveSpeed * delta
    this.GameObject.WorldPosition = position
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(0, 2)
end