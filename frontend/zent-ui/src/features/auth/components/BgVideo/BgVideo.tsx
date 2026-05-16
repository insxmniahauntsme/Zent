import { useRef } from "react";

export interface BgVideoProps {
  src: string;
  className?: string;
}

const BgVideo = ({ src, className }: BgVideoProps) => {
  const videoRef = useRef<HTMLVideoElement>(null);

  const handleLoaded = () => {
    if (videoRef.current) {
      videoRef.current.playbackRate = 0.8;
    }
  };

  return (
    <video
      ref={videoRef}
      className={className}
      src={src}
      autoPlay
      loop
      muted
      playsInline
      onLoadedMetadata={handleLoaded}
    />
  );
};

export default BgVideo;
