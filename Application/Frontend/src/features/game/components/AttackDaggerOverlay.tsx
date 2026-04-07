import DaggerImage from "@/assets/images/dagger.png";
import { useEffect, useMemo, useRef, useState } from "react";

export const DAGGER_FLIGHT_MS = 360;
export const DAGGER_STUCK_MS = 280;

export interface AttackDaggerAnimation {
  id: number;
  startX: number;
  startY: number;
  endX: number;
  endY: number;
  attackFailed: boolean;
}

interface AttackDaggerOverlayProps {
  animation: AttackDaggerAnimation | null;
  selectedAttackerCardId?: string | null;
  onAnimationEnd?: () => void;
}

const getElementCenter = (selector: string) => {
  const element = document.querySelector<HTMLElement>(selector);
  if (!element) return null;

  const rect = element.getBoundingClientRect();
  return {
    x: rect.left + rect.width / 2,
    y: rect.top + rect.height / 2,
  };
};

const getDaggerRotationDeg = (startX: number, startY: number, endX: number, endY: number) =>
  (Math.atan2(endY - startY, endX - startX) * 180) / Math.PI + 90;

const AttackDaggerOverlay = ({
  animation,
  selectedAttackerCardId,
  onAnimationEnd,
}: AttackDaggerOverlayProps) => {
  const [phase, setPhase] = useState<"idle" | "flying" | "stuck">("idle");
  const [mousePosition, setMousePosition] = useState<{ x: number; y: number } | null>(null);
  const onAnimationEndRef = useRef(onAnimationEnd);

  useEffect(() => {
    onAnimationEndRef.current = onAnimationEnd;
  }, [onAnimationEnd]);

  useEffect(() => {
    const onMouseMove = (event: MouseEvent) => {
      setMousePosition({ x: event.clientX, y: event.clientY });
    };

    window.addEventListener("mousemove", onMouseMove);
    return () => {
      window.removeEventListener("mousemove", onMouseMove);
    };
  }, []);

  useEffect(() => {
    if (!animation) return undefined;

    setPhase("idle");
    let stuckTimer: ReturnType<typeof setTimeout> | null = null;

    const startTimer = setTimeout(() => {
      setPhase("flying");
    }, 0);

    const flightTimer = setTimeout(() => {
      if (animation.attackFailed) {
        onAnimationEndRef.current?.();
        return;
      }

      setPhase("stuck");
      stuckTimer = setTimeout(() => {
        onAnimationEndRef.current?.();
      }, DAGGER_STUCK_MS);
    }, DAGGER_FLIGHT_MS);

    return () => {
      clearTimeout(startTimer);
      clearTimeout(flightTimer);
      if (stuckTimer) clearTimeout(stuckTimer);
    };
  }, [animation]);

  const style = useMemo(() => {
    if (!animation) return null;

    const dx = animation.endX - animation.startX;
    const dy = animation.endY - animation.startY;
    const progress = phase === "idle" ? 0 : 1;
    const rotationDeg = getDaggerRotationDeg(
      animation.startX,
      animation.startY,
      animation.endX,
      animation.endY
    );

    return {
      left: animation.startX,
      top: animation.startY,
      transform: `translate(-50%, -50%) translate(${dx * progress}px, ${dy * progress}px) rotate(${rotationDeg}deg)`,
      transition:
        phase === "flying"
          ? `transform ${DAGGER_FLIGHT_MS}ms cubic-bezier(0.22, 0.8, 0.26, 1)`
          : undefined,
      clipPath: phase === "stuck" ? "inset(50% 0 0 0)" : "none",
    } as const;
  }, [animation, phase]);

  const aimPreviewStyle = useMemo(() => {
    if (animation || !selectedAttackerCardId || !mousePosition) return null;

    const attackerCenter = getElementCenter(`[data-game-card-id="${selectedAttackerCardId}"]`);
    if (!attackerCenter) return null;

    const rotationDeg = getDaggerRotationDeg(
      attackerCenter.x,
      attackerCenter.y,
      mousePosition.x,
      mousePosition.y
    );

    return {
      left: attackerCenter.x,
      top: attackerCenter.y,
      transform: `translate(-50%, -50%) rotate(${rotationDeg}deg)`,
    } as const;
  }, [animation, mousePosition, selectedAttackerCardId]);

  if (!style && !aimPreviewStyle) return null;

  return (
    <div className="pointer-events-none fixed inset-0 z-[70]">
      {aimPreviewStyle ? (
        <img
          src={DaggerImage}
          alt="Attack dagger aim"
          style={aimPreviewStyle}
          className="fixed h-[74px] w-[28px] select-none opacity-85 [filter:drop-shadow(0_0_10px_rgba(251,191,36,0.55))]"
          draggable={false}
        />
      ) : null}
      {style ? (
        <img
          src={DaggerImage}
          alt="Attack dagger"
          style={style}
          className="fixed h-[74px] w-[28px] select-none [filter:drop-shadow(0_0_10px_rgba(248,113,113,0.6))]"
          draggable={false}
        />
      ) : null}
    </div>
  );
};

export default AttackDaggerOverlay;
