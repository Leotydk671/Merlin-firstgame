using UnityEngine;

public class SpellLogical_Test : SpellLogical, LogicalOperators
{
    public ParticleSystem Emit_Particle = null;

    private float last_time = 0.5f;

    public SpellLogical_Test()
    {
        Emit_Particle = Object.Instantiate(Resources.Load<ParticleSystem>("Prefabs/Particle/Exploding"));
        if(Emit_Particle == null)
        {
            Debug.Log("Particle is null");
        }
        else
        {
            Debug.Log("Particle is done");
        }

    }

    //---子弹发射时调用：将粒子系统移动到子弹位置并播放爆炸特效---
    public override void Emit(BulletBase Hanging_bullet_base)
    {
        if(Hanging_bullet_base != null)
        {
            if(Hanging_bullet_base is EntityBullet Hanging_bullet)
            {
                Emit_Particle.transform.position = Hanging_bullet.gameObject.transform.position;
                Debug.Log("Play particle");
                Emit_Particle.Play();
            }
        }
        
    }

    //---子弹每帧处理：每0.1秒切换子弹的透明度实现闪烁视觉效果---
    public override void Process(BulletBase Hanging_bullet_base)
    {
        if(Hanging_bullet_base != null)
        {
            if(Hanging_bullet_base is EntityBullet Hanging_bullet)
            {
                SpriteRenderer sr = Hanging_bullet.GetComponent<SpriteRenderer>();
                if(sr == null)
                {
                    Debug.Log("SR is null!");
                    return;
                }
                Color Flashing_Color1 = sr.color;
            
                if(Time.realtimeSinceStartup - last_time > 0.1f)
                {
                    if(Flashing_Color1.a != 1.0f)
                    {
                        Flashing_Color1.a = 1.0f;
                    }
                    else
                    {
                        Flashing_Color1.a = 0.1f;
                    }
                    sr.color = Flashing_Color1;
                    Debug.Log("Color update sr" + sr.GetInstanceID());
                    Debug.Log("Color update object" + Hanging_bullet.GetInstanceID());
                    last_time = Time.realtimeSinceStartup;
                }
            }

        }
        
    }

    //---子弹命中时调用：在命中位置播放爆炸粒子特效---
    public override void Hit(Collider2D Colliding_bullet, BulletBase Hanging_bullet_base)
    {
        if(Hanging_bullet_base != null)
        {
            if(Hanging_bullet_base is EntityBullet Hanging_bullet)
            {
                Emit_Particle.transform.position = Hanging_bullet.transform.position;
                Emit_Particle.Play();
            }
        }
    }

    //---法术销毁时重置闪烁计时器---
    public void Spell_Destroy()
    {
        last_time = 0;
    }

}
