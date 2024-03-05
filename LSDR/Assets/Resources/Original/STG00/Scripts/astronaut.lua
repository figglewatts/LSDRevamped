moveSpeed = 0.2

function start()
    this.PlayAnimation(0)
    
    if not IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    local delta = Unity.DeltaTime()
    local position = this.GameObject.WorldPosition
    position.x = position.x - moveSpeed * delta
    this.GameObject.WorldPosition = position
end

function interact()
    this.LogGraphContribution(0, 2, this)
end